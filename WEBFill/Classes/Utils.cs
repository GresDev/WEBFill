using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WEBFill.Classes
{
    public static class Utils
    {

        public static Dictionary<string, string> Titles;
        public static Dictionary<string, string> Anons;
        public static Dictionary<string, string> Presenters;
        public static Dictionary<string, string> Directors;
        public static Dictionary<string, DirectorsSchedule> DirectorsSchedule;

        /// <summary>
        /// Заполняет словари данными из выбранного файла Excel
        /// </summary>
        /// <returns></returns>
        public static async Task GetDictionariesAsync()
        {
            Titles = await ExcelInteraction.GetDictionaryAsync("Titles");
            Anons = await ExcelInteraction.GetDictionaryAsync("Anons");
            Presenters = await ExcelInteraction.GetDictionaryAsync("Presenters");
            Directors = await ExcelInteraction.GetDictionaryAsync("Directors");
            DirectorsSchedule = await ExcelInteraction.GetDirectorScheduleDictionaryAsync("DirectorsSchedule");
        }

        /// <summary>
        /// Возвращает список имен файлов *.mp3, находящихся в целевой папке и отсутствующих в открытой таблице Excel
        /// </summary>
        /// <param name="pathToFiles"></param>
        /// <param name="broadcastsToSend"></param>
        /// <returns></returns>
        public static List<string> LoadMediaFilesList(string pathToFiles, List<Broadcast> broadcastsToSend)
        {
            string[] totalMediaFileList = Directory.GetFiles(pathToFiles, "*.mp3");
            for (int i = 0; i < totalMediaFileList.Length; i++)
            {
                totalMediaFileList[i] = Path.GetFileNameWithoutExtension(totalMediaFileList[i]);
            }

            List<string> fileList = new List<string>();

            foreach (var fileName in totalMediaFileList)
            {
                bool fileIsNew = true;

                foreach (var broadcast in broadcastsToSend)
                {
                    if ((fileName + ".mp3") == broadcast.FileName)
                    {
                        fileIsNew = false;
                        break;
                    }
                }

                if (fileIsNew)
                {
                    fileList.Add(fileName);
                }
            }

            return fileList;
        }

        /// <summary>
        /// Возвращает строку, содержащую полные имена авторов указанного выпуска передачи
        /// </summary>
        /// <param name="presentersString"></param>
        /// <param name="presenters"></param>
        /// <returns></returns>
        public static string ParsePresenters(string fileName, string presentersString)
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
                        MessageBox.Show($"Псевдоним \"{presenterName}\" не найден в списке авторов!\n" + $"Имя файла: \"{fileName}.mp3\"", "Псевдоним автора не найден в списке!", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        /// <summary>
        /// Возвращает массив строк, содержащих информацию о наименовании передачи, дате и времени выхода в эфир
        /// </summary>
        /// <param name="mediaFileName"></param>
        /// <param name="currentYear"></param>
        /// <returns></returns>
        public static string[] ParseFileName(string mediaFileName, string currentYear)
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

                        //parsedString[2] = dateArray[0] + "." + dateArray[1] + "." + currentYear;
                        parsedString[2] = dateArray[0] + "." + dateArray[1] + "." + "20" + dateArray[2];
                    }
                    else
                    {
                        parsedString[0] = "combined";
                        string[] combinedDate = dateArray[0].Split(new char[] { '-' });
                        //parsedString[2] = combinedDate[0] + "." + dateArray[1] + "." + currentYear;
                        //parsedString[4] = combinedDate[1] + "." + dateArray[1] + "." + currentYear;
                        parsedString[2] = combinedDate[0] + "." + dateArray[1] + "." + "20" + dateArray[2];
                        parsedString[4] = combinedDate[1] + "." + dateArray[1] + "." + "20" + dateArray[2];
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

        /// <summary>
        /// Возвращает полное имя звукорежиссера для заданного выпуска передачи на основе времени выхода в эфир
        /// </summary>
        /// <param name="bCast"></param>
        /// <param name="directors"></param>
        /// <param name="directorsSchedule"></param>
        /// <returns></returns>
        public static string GetDirector(Broadcast bCast, Dictionary<string, string> directors, Dictionary<string, DirectorsSchedule> directorsSchedule)
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
                    return directors[directorsSchedule[broadCast.Date].Interval1.ToLower()];
                }

                if (TimeSpan.Parse(broadCast.Time) > clock08 & TimeSpan.Parse(broadCast.Time) < clock15)
                {
                    return directors[directorsSchedule[broadCast.Date].Interval2.ToLower()];
                }

                if (TimeSpan.Parse(broadCast.Time) > clock15 & TimeSpan.Parse(broadCast.Time) < clock22)
                {
                    return directors[directorsSchedule[broadCast.Date].Interval3.ToLower()];
                }

                if (TimeSpan.Parse(broadCast.Time) > clock22 & TimeSpan.Parse(broadCast.Time) < clock23)
                {
                    return directors[directorsSchedule[broadCast.Date].Interval4.ToLower()];
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

            return string.Empty;
        }

    }

}
