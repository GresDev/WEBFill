using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace WEBFill.Classes
{
    class BroadcastDB
    {
        public string DBFileName { get; set; }

        private const string CurrentYear = "2020";

        public List<Broadcast> BroadcastList { get; set; }
        public string[] MediaFilesList { get; set; }
        public Dictionary<string, string> Titles { get; set; }
        public Dictionary<string, string> Anons { get; set; }
        public Dictionary<string, string> Presenters { get; set; }
        public Dictionary<string, string> Authors { get; set; }
        public Dictionary<string, string> Directors { get; set; }
        public Dictionary<string, DirectorsShedule> DShedule { get; set; }


        public BroadcastDB()
        {
        }

        public BroadcastDB(string BroadcastFileName, ProgressBar progressBar)
        {
            DBFileName = BroadcastFileName;

            GetDictionaries();

            MediaFilesList = LoadMediaFilesList("mp3");

            BroadcastList = GetBroadcastList();

            ExcelInteraction.BroadcastTableFill(DBFileName, BroadcastList, progressBar);
        }

        public BroadcastDB(string BroadcastFileName)
        {
            DBFileName = BroadcastFileName;

            Presenters = ExcelInteraction.GetDictionary(DBFileName, "Presenters");

            List<BroadcastAuthor> BroadcastAutorsList = ExcelInteraction.GetAuthors(BroadcastFileName);
            List<BroadcastAuthor> BroadcastParsedAutorsList = new List<BroadcastAuthor>();

            foreach (BroadcastAuthor author in BroadcastAutorsList)
            {
                author.NameString = ParseAuthors(author.NameString);
                BroadcastParsedAutorsList.Add(author);
            }

            ExcelInteraction.UpdateAuthors(DBFileName, BroadcastParsedAutorsList);
            _ = MessageBox.Show("Авторы обновлены", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private List<Broadcast> GetBroadcastList()
        {
            List<Broadcast> broadcastList = new List<Broadcast>();
            int i = 1;
            foreach (string mediaFileName in MediaFilesList)
            {
                string[] mediaInfo = ParseFileName(mediaFileName);


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
                    Environment.Exit(-1);
                }

                broadcast.SetDefaultValues();

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


                if (broadcast.Author == "-")
                {
                    broadcast.Author = Authors[broadcast.Title.ToLower()];
                    broadcast.Presenters = broadcast.Author;
                }

                broadcast = SetDirector(broadcast);

                broadcastList.Add(broadcast);
                i++;
            }

            return broadcastList;
        }

        private Broadcast SetDirector(Broadcast broadcast)
        {
            Broadcast _broadcast = broadcast;

            var Clock_0 = new TimeSpan(0, 0, 0);
            var Clock_8 = new TimeSpan(8, 0, 0);
            var Clock_15 = new TimeSpan(15, 0, 0);
            var Clock_22 = new TimeSpan(22, 0, 0);
            var Clock_23 = new TimeSpan(23, 59, 59);

            //try
            //{
                if (TimeSpan.Parse(_broadcast.Time) > Clock_0 & TimeSpan.Parse(_broadcast.Time) < Clock_8)
                {
                    _broadcast.Director = Directors[DShedule[_broadcast.Date].Interval1.ToLower()];
                }

                if (TimeSpan.Parse(_broadcast.Time) > Clock_8 & TimeSpan.Parse(_broadcast.Time) < Clock_15)
                {
                    _broadcast.Director = Directors[DShedule[_broadcast.Date].Interval2.ToLower()];
                }

                if (TimeSpan.Parse(_broadcast.Time) > Clock_15 & TimeSpan.Parse(_broadcast.Time) < Clock_22)
                {
                    _broadcast.Director = Directors[DShedule[_broadcast.Date].Interval3.ToLower()];
                }

                if (TimeSpan.Parse(_broadcast.Time) > Clock_22 & TimeSpan.Parse(_broadcast.Time) < Clock_23)
                {
                    _broadcast.Director = Directors[DShedule[_broadcast.Date].Interval4.ToLower()];
                }
            //}
            //catch (KeyNotFoundException)
            //{
            //    MessageBox.Show($"Звукорежиссер для {_broadcast.Title} за {_broadcast.Date} число не найден!\n" +
            //                    $"Вероятные причины:\n" +
            //                    $"1. Неверно указана дата выхода программы в имени файла.\n" +
            //                    "2. Незаполнено или неправильно заполнено расписание звукорежиссеров.\n" +
            //                    $"3. На вкладке \"DirectorsShedule\" в фамилиях звукорежиссеров присутствуют лишние пробелы в начале и/или в конце строк.\n" + 
            //                    $"4. Появился новый звукорежиссёр, ранее не указанный в перечне.");
            //    Environment.Exit(-1);
            //}

            return _broadcast;
        }


        private void GetDictionaries()
        {
            Titles = ExcelInteraction.GetDictionary(DBFileName, "Titles");
            Anons = ExcelInteraction.GetDictionary(DBFileName, "Anons");
            Presenters = ExcelInteraction.GetDictionary(DBFileName, "Presenters");
            Authors = ExcelInteraction.GetDictionary(DBFileName, "Authors");
            Directors = ExcelInteraction.GetDictionary(DBFileName, "Directors");
            DShedule = ExcelInteraction.GetDirectorSheduleDictionary(DBFileName, "DirectorsShedule");
        }

        private string[] LoadMediaFilesList(string PathToFiles)
        {
            string[] MediaFileList = Directory.GetFiles(PathToFiles, "*.mp3");
            for (int i = 0; i < MediaFileList.Length; i++)
            {
                MediaFileList[i] = Path.GetFileNameWithoutExtension(MediaFileList[i]);
            }

            return MediaFileList;
        }

        private string ParseAuthors(string AuthorsString)
        {
            string[] parsedString = AuthorsString.Split(new char[] {',', ' '});

            List<string> authorsList = new List<string>();

            foreach (string authorName in parsedString)
            {
                if (Presenters.ContainsKey(authorName.ToLower()))
                {
                    authorsList.Add(authorName);
                }
                else
                {
                    if (!string.IsNullOrEmpty(authorName))
                    {
                        MessageBox.Show($"Псевдоним \"{authorName}\" не найден в списке авторов!");
                        Environment.Exit(0);
                    }
                }
            }


            string[] authorsArray = authorsList.ToArray();

            string resultString = Presenters[authorsArray[0].ToLower()];

            int i = 1;
            while (i < authorsArray.Length)
            {
                if (Presenters.ContainsKey(authorsArray[i].ToLower()))
                {
                    resultString += ", " + Presenters[authorsArray[i].ToLower()];
                }

                i++;
            }

            return resultString;
        }

        private string[] ParseFileName(string MediaFileName)
        {
            string[] parsedStringRaw = MediaFileName.Split(new char[] {' '});
            string[] parsedString = new string[7];
            //parsedString[5] = "-";

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
                    string[] dateArray = parsedStringRaw[j].Split(new char[] {'_'});
                    if (!dateArray[0].Contains("-"))
                    {
                        parsedString[0] = "single";

                        //--------------

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

                        //--------------

                        parsedString[2] = dateArray[0] + "." + dateArray[1] + "." + CurrentYear;
                    }
                    else
                    {
                        parsedString[0] = "combined";
                        string[] combinedDate = dateArray[0].Split(new char[] {'-'});
                        parsedString[2] = combinedDate[0] + "." + dateArray[1] + "." + CurrentYear;
                        parsedString[4] = combinedDate[1] + "." + dateArray[1] + "." + CurrentYear;
                    }

                    string[] timeArray = parsedStringRaw[j + 1].Split(new char[] {'_'});
                    parsedString[3] = timeArray[0] + ":" + timeArray[1] + ":" + timeArray[2];

                    if (parsedStringRaw.Length > j + 2)
                    {
                        //parsedString[5] = parsedStringRaw[j + 2];
                        for (int k = j + 2; k < parsedStringRaw.Length; k++)
                        {
                            parsedString[5] = parsedString[5] + " " + parsedStringRaw[k];
                        }
                    }
                    else
                    {
                        parsedString[5] = "-";
                    }

                    parsedString[6] = MediaFileName + ".mp3";

                    break;
                }
            }

            return parsedString;
        }
    }
}