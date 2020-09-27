using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace WEBFill.Classes
{
    class BroadcastDb
    {
        public string DbFileName { get; set; }

        private const string CurrentYear = "2020";

        public List<Broadcast> BroadcastList { get; set; }
        public string[] MediaFilesList { get; set; }
        public Dictionary<string, string> Titles { get; set; }
        public Dictionary<string, string> Anons { get; set; }
        public Dictionary<string, string> Presenters { get; set; }
        public Dictionary<string, string> Directors { get; set; }
        public Dictionary<string, DirectorsShedule> DirectorsSchedule { get; set; }

        public BroadcastDb(string broadcastFileName, ProgressBar progressBar)
        {
            DbFileName = broadcastFileName;

            GetDictionaries();

            MediaFilesList = LoadMediaFilesList("mp3");

            BroadcastList = GetBroadcastList();

            ExcelInteraction.BroadcastTableFill(DbFileName, BroadcastList, progressBar);
        }

        private List<Broadcast> GetBroadcastList()
        {
            List<Broadcast> broadcastList = new List<Broadcast>();

            int i = 1;

            foreach (string mediaFileName in MediaFilesList)
            {
                string[] mediaInfo = ParseFileName(mediaFileName);

                mediaInfo[5] = ParsePresenters(mediaInfo[5]);

                Broadcast broadcast = new Broadcast()
                {
                    Id = i,
                    DateAired = mediaInfo[2] + " " + mediaInfo[3],
                    Date = mediaInfo[2],
                    Time = mediaInfo[3],
                    Author = mediaInfo[5],
                    Presenters = mediaInfo[5],
                    FileName = mediaInfo[6],
                    FileExists = true
                };

                try
                {
                    broadcast.Title = Titles[mediaInfo[1].ToLower()];
                }
                catch (KeyNotFoundException)
                {
                    MessageBox.Show($"Программа \"{mediaInfo[1]}\" не найдена в перечне!");
                    Process.GetCurrentProcess().Kill();
                }

                if (mediaInfo[0] == "combined")
                {
                    broadcast.DateAiredEnd = mediaInfo[4];
                }

                broadcast.Anons = Anons[broadcast.Title.ToLower()];
                if (broadcast.DateAiredEnd == "-")
                {
                    broadcast.Fragments = "В прилагаемом файле выпуск программы от " + broadcast.Date + ".";
                }
                else
                {
                    broadcast.Fragments = "В прилагаемом файле выпуски программы, вышедшие с " + mediaInfo[2] + " по " +
                                          mediaInfo[4];
                }

                broadcast = SetDirector(broadcast);

                broadcast.Sha256 = Sha256.GetHash(@".\MP3\" + mediaFileName + ".mp3");



                broadcastList.Add(broadcast);
                i++;
            }

            return broadcastList;
        }

        private Broadcast SetDirector(Broadcast bCast)
        {
            Broadcast broadCast = bCast;

            var clock00 = new TimeSpan(0, 0, 0);
            var clock08 = new TimeSpan(8, 0, 0);
            var clock15 = new TimeSpan(15, 0, 0);
            var clock22 = new TimeSpan(22, 0, 0);
            var clock23 = new TimeSpan(23, 59, 59);

            try
            {
                if (TimeSpan.Parse(broadCast.Time) > clock00 & TimeSpan.Parse(broadCast.Time) < clock08)
                {
                    broadCast.Director = Directors[DirectorsSchedule[broadCast.Date].Interval1.ToLower()];
                }

                if (TimeSpan.Parse(broadCast.Time) > clock08 & TimeSpan.Parse(broadCast.Time) < clock15)
                {
                    broadCast.Director = Directors[DirectorsSchedule[broadCast.Date].Interval2.ToLower()];
                }

                if (TimeSpan.Parse(broadCast.Time) > clock15 & TimeSpan.Parse(broadCast.Time) < clock22)
                {
                    broadCast.Director = Directors[DirectorsSchedule[broadCast.Date].Interval3.ToLower()];
                }

                if (TimeSpan.Parse(broadCast.Time) > clock22 & TimeSpan.Parse(broadCast.Time) < clock23)
                {
                    broadCast.Director = Directors[DirectorsSchedule[broadCast.Date].Interval4.ToLower()];
                }
            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show($"Звукорежиссер для {broadCast.Title} за {broadCast.Date} число не найден!\n" +
                                $"Вероятные причины:\n" +
                                $"1. Неверно указана дата выхода программы в имени файла.\n" +
                                "2. Незаполнено или неправильно заполнено расписание звукорежиссеров.\n" +
                                $"3. Появился новый звукорежиссёр, ранее не указанный в перечне.");
                Process.GetCurrentProcess().Kill();
            }

            return broadCast;
        }

        private void GetDictionaries()
        {
            Titles = ExcelInteraction.GetDictionary(DbFileName, "Titles");
            Anons = ExcelInteraction.GetDictionary(DbFileName, "Anons");
            Presenters = ExcelInteraction.GetDictionary(DbFileName, "Presenters");
            Directors = ExcelInteraction.GetDictionary(DbFileName, "Directors");
            DirectorsSchedule = ExcelInteraction.GetDirectorSheduleDictionary(DbFileName, "DirectorsShedule");
        }

        private string[] LoadMediaFilesList(string pathToFiles)
        {
            string[] mediaFileList = Directory.GetFiles(pathToFiles, "*.mp3");
            for (int i = 0; i < mediaFileList.Length; i++)
            {
                mediaFileList[i] = Path.GetFileNameWithoutExtension(mediaFileList[i]);
            }

            return mediaFileList;
        }

        private string ParsePresenters(string presentersString)
        {
            string[] parsedString = presentersString.Split(new char[] { ',', ' ' });

            List<string> presentersList = new List<string>();

            foreach (string presenterName in parsedString)
            {
                if (Presenters.ContainsKey(presenterName.ToLower()))
                {
                    presentersList.Add(presenterName);
                }
                else
                {
                    if (!string.IsNullOrEmpty(presenterName))
                    {
                        MessageBox.Show($"Псевдоним \"{presenterName}\" не найден в списке авторов!", "Пропущен псевдоним!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Process.GetCurrentProcess().Kill();
                    }
                }
            }


            string[] presentersArray = presentersList.ToArray();

            string resultString = Presenters[presentersArray[0].ToLower()];

            int i = 1;
            while (i < presentersArray.Length)
            {
                if (Presenters.ContainsKey(presentersArray[i].ToLower()))
                {
                    resultString += ", " + Presenters[presentersArray[i].ToLower()];
                }

                i++;
            }

            return resultString;
        }

        private string[] ParseFileName(string mediaFileName)
        {
            string[] parsedStringRaw = mediaFileName.Split(new char[] { ' ' });
            string[] parsedString = new string[7];

            int i = 1;
            parsedString[1] = parsedStringRaw[0];

            while (!parsedStringRaw[i].Contains("_"))
            {
                parsedString[1] = parsedString[1] + " " + parsedStringRaw[i];
                i++;
            }

            for (int j = i; j < parsedStringRaw.Length; j++)
            {
                if (parsedStringRaw[j].Contains("_"))
                {
                    string[] dateArray = parsedStringRaw[j].Split(new char[] { '_' });
                    if (!dateArray[0].Contains("-"))
                    {
                        parsedString[0] = "single";

                        if (Int32.TryParse(dateArray[0], out int date))
                        {
                            if ((date < 10) && (dateArray[0].Length == 1))
                            {
                                dateArray[0] = "0" + dateArray[0];
                            }
                        }
                        else
                        {
                            MessageBox.Show("Некорректная дата!");
                        }

                        parsedString[2] = dateArray[0] + "." + dateArray[1] + "." + CurrentYear;
                    }
                    else
                    {
                        parsedString[0] = "combined";
                        string[] combinedDate = dateArray[0].Split(new char[] { '-' });
                        parsedString[2] = combinedDate[0] + "." + dateArray[1] + "." + CurrentYear;
                        parsedString[4] = combinedDate[1] + "." + dateArray[1] + "." + CurrentYear;
                    }

                    string[] timeArray = parsedStringRaw[j + 1].Split(new char[] { '_' });
                    if (timeArray.Length != 3)
                    {
                        MessageBox.Show(mediaFileName, "Неправильный формат времени выхода передачи!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Process.GetCurrentProcess().Kill();
                    }
                    parsedString[3] = timeArray[0] + ":" + timeArray[1] + ":" + timeArray[2];

                    if (parsedStringRaw.Length > j + 2)
                    {
                        for (int k = j + 2; k < parsedStringRaw.Length; k++)
                        {
                            parsedString[5] = parsedString[5] + " " + parsedStringRaw[k];
                        }
                    }
                    else
                    {
                        parsedString[5] = "-";
                    }

                    parsedString[6] = mediaFileName + ".mp3";

                    break;
                }
            }

            return parsedString;
        }
    }
}