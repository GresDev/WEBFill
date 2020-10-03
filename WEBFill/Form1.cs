using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WEBFill.Classes;

namespace WEBFill
{
    public partial class Form1 : Form
    {

        private delegate void LoginHandler();

        /// <summary>
        /// Происходит при успешной авторизации пользоваеля на сайте http://oed.gtrf.ru/
        /// </summary>
        private event LoginHandler LoginNotify;

        private const string CurrentYear = "2020";

        private bool _userLoggedIn;

        private bool _excelLoaded;

        private WebForm _webForm;

        private bool _newPageLoaded;

        private readonly string _currentDir = Application.StartupPath + @"\mp3\";

        private List<string> _mediaFilesList;

        private List<Broadcast> _broadcastsToSend = new List<Broadcast>();

        private Dictionary<string, string> _titles;
        private Dictionary<string, string> _anons;
        private Dictionary<string, string> _presenters;
        private Dictionary<string, string> _directors;
        private Dictionary<string, DirectorsShedule> _directorsSchedule;

        private string _sourceHtmlText;

        public Form1()
        {
            InitializeComponent();
            webBrowserGTRF.DocumentCompleted += PageLoaded;
            LoginNotify += SetAuthSuccessControls;

            LoadCaptchaImage();

            if (CheckForAuthSuccess())
            {
                LoginNotify?.Invoke();
            }

            _webForm = new WebForm(webBrowserGTRF, captchaTextBox.Text);
        }

        private void AuthButton_Click(object sender, EventArgs e)
        {

            _webForm = new WebForm(webBrowserGTRF, captchaTextBox.Text);
            _webForm.WebFormAuth(userNameTextBox.Text, userPasswordTextBox.Text);

            _newPageLoaded = false;
            while (_newPageLoaded == false)
            {
                Application.DoEvents();
            }
            _newPageLoaded = false;

            if (!CheckForAuthSuccess())
            {
                webBrowserGTRF.Navigate("http://oed.gtrf.ru/auth");

                _newPageLoaded = false;
                while (_newPageLoaded == false)
                {
                    Application.DoEvents();
                }
                _newPageLoaded = false;

                MessageBox.Show(
                    "Проверьте правильность заполнения данных аутентификации и правильность ввода кода с картинки", "Ошибка аутентификации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                captchaTextBox.Text = String.Empty;
                LoadCaptchaImage();
                return;
            }

            LoginNotify?.Invoke();

            if (_excelLoaded)
            {
                startSendingButton.Enabled = true;
            }

        }

        private async void LoadFromExcelButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            _broadcastsToSend = await ExcelInteraction.LoadListFromExcelAsync(openExcelFileDialog, selectedExcelFileNameLabel);
            if (_broadcastsToSend == null)
            {
                return;
            }

            _excelLoaded = true;

            await GetDictionariesAsync();

            if (_userLoggedIn && _broadcastsToSend.Any())
            {
                startSendingButton.Enabled = true;
                createBroadcastTableButton.Enabled = true;
            }

            if (!_broadcastsToSend.Any() || !_userLoggedIn)
            {
                createBroadcastTableButton.Enabled = true;
                startSendingButton.Enabled = false;
            }

            broadcastTotalNumberLabel.Text = _broadcastsToSend.Count().ToString();
            broadcastTransmittedNumberLabel.Text = _broadcastsToSend.Count(x => x.Transmitted == "1").ToString();

        }

        private void StartSendingButton_Click(object sender, EventArgs e)
        {

            startSendingButton.Enabled = false;
            loadFromExcelButton.Enabled = false;
            createBroadcastTableButton.Enabled = false;

            int i = 0;
            foreach (var broadcast in _broadcastsToSend)
            {
                if (!broadcast.FileExists)
                {
                    MessageBox.Show($"Отсутствует файл {broadcast.FileName} для передачи {broadcast.Title}.", "Файл не найден!", MessageBoxButtons.OK);
                    continue;
                }

                if (broadcast.Transmitted == "1")
                {
                    continue;
                }

                _newPageLoaded = false;
                webBrowserGTRF.Navigate("http://oed.gtrf.ru/materials/edit");
                while (_newPageLoaded == false)
                {
                    Application.DoEvents();
                }
                _newPageLoaded = false;

                _webForm.FillWebForm(webBrowserGTRF, broadcast);
                while (_newPageLoaded == false)
                {
                    Application.DoEvents();
                }
                _newPageLoaded = false;

                webBrowserGTRF.Document?.GetElementById("fileUpload")?.Focus();
                SendKeys.Send(" " + _currentDir + broadcast.FileNameFormat());
                SendKeys.SendWait(" \n");
                foreach (HtmlElement input in webBrowserGTRF.Document.GetElementsByTagName("button"))
                {
                    if (input.GetAttribute("InnerText") == " Загрузить файл")
                    {
                        input.InvokeMember("click");
                    }
                }
                while (_newPageLoaded == false)
                {
                    Application.DoEvents();
                }
                _newPageLoaded = false;

                foreach (HtmlElement input in webBrowserGTRF.Document.GetElementsByTagName("button"))
                {
                    if (input.GetAttribute("InnerText") == "Сохранить и отправить")
                    {
                        input.InvokeMember("click");
                    }
                }

                while (_newPageLoaded == false)
                {
                    Application.DoEvents();
                }
                _newPageLoaded = false;
                ExcelInteraction.SetTransmittedFlag(ExcelInteraction.ExcelFileName, broadcast);
                i++;

            }
            webBrowserGTRF.Navigate("http://oed.gtrf.ru/");
            MessageBox.Show($"Передача файлов завершена\nВсего передано выпусков программ: {i}", "Завершено!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PageLoaded(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            _newPageLoaded = true;
        }

        private async void CreateBroadcastTableButton_Click(object sender, EventArgs e)
        {

            loadFromExcelButton.Enabled = false;

            startSendingButton.Enabled = false;

            createBroadcastTableButton.Enabled = false;

            _mediaFilesList = LoadMediaFilesList(_currentDir);

            int id = 0;

            foreach (var broadcast in _broadcastsToSend)
            {
                if (broadcast.Id > id)
                {
                    id = broadcast.Id;
                }
            }

            int count = 0;

            foreach (var fileName in _mediaFilesList)
            {
                string[] mediaInfo = ParseFileName(fileName);

                mediaInfo[5] = ParsePresenters(mediaInfo[5]);

                Broadcast broadcast = new Broadcast()
                {
                    Id = ++id,
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
                    broadcast.Title = _titles[mediaInfo[1].ToLower()];
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

                broadcast.Anons = _anons[broadcast.Title.ToLower()];
                if (broadcast.DateAiredEnd == "-")
                {
                    broadcast.Fragments = "В прилагаемом файле выпуск программы от " + broadcast.Date + ".";
                }
                else
                {
                    broadcast.Fragments = "В прилагаемом файле выпуски программы, вышедшие с " + mediaInfo[2] + " по " +
                                          mediaInfo[4];
                }

                broadcast.Director = GetDirector(broadcast);

                broadcast.Sha256 = await Task.Run(() => Sha256.GetHash(@".\MP3\" + fileName + ".mp3"));

                _broadcastsToSend.Add(broadcast);

                broadcastTotalNumberLabel.Text = _broadcastsToSend.Count().ToString();

                await ExcelInteraction.WriteBroadcastToTableAsync(broadcast);

                count++;

                progressBar.Value = count * 100 / _mediaFilesList.Count();

                try
                {
                    richTextBox1.Text += broadcast.Id + ": " + broadcast.Title + " (" + broadcast.Date + ")\n";
                    richTextBox1.Select(richTextBox1.Text.Length, 0);
                    richTextBox1.ScrollToCaret();
                }
                catch (ObjectDisposedException)
                {

                }
            }

            try
            {
                richTextBox1.Text += "\nТаблица заполнена!\n";
                richTextBox1.Select(richTextBox1.Text.Length, 0);
                richTextBox1.ScrollToCaret();
            }
            catch (ObjectDisposedException)
            {

            }

            MessageBox.Show("Таблица заполнена!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (_broadcastsToSend.Any())
            {
                startSendingButton.Enabled = true;
            }

            loadFromExcelButton.Enabled = true;
        }

        private void ReloadCaptchaButton_Click(object sender, EventArgs e)
        {
            captchaTextBox.Text = String.Empty;
            LoadCaptchaImage();
        }

        private void SetAuthButtonState()
        {
            if (!string.IsNullOrEmpty(captchaTextBox.Text) && !string.IsNullOrEmpty(userNameTextBox.Text) &&
                !string.IsNullOrEmpty(userPasswordTextBox.Text))
            {
                authButton.Enabled = true;
            }
            else
            {
                authButton.Enabled = false;
            }
        }
        private void UserNameTextBox_TextChanged(object sender, EventArgs e)
        {
            SetAuthButtonState();
        }

        private void UserPasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            SetAuthButtonState();
        }

        private void CaptchaTextBox_TextChanged(object sender, EventArgs e)
        {
            SetAuthButtonState();
        }

        /// <summary>
        /// Загрузка изображения CAPTCHA
        /// </summary>
        private void LoadCaptchaImage()
        {
            webBrowserGTRF.Url = new Uri("http://oed.gtrf.ru/auth");
            webBrowserGTRF.ScriptErrorsSuppressed = true;

            while (_newPageLoaded == false)
            {
                Application.DoEvents();
            }
            _newPageLoaded = false;

            _sourceHtmlText = webBrowserGTRF.DocumentText;
            var offset = _sourceHtmlText.IndexOf("/captcha/", StringComparison.Ordinal);
            var s = _sourceHtmlText.Substring(offset + 9, 36);
            captchaPictureBox.ImageLocation = "http://oed.gtrf.ru/captcha/" + s;
        }

        /// <summary>
        /// Возвращает true, если пользователь авторизован, иначе false
        /// </summary>
        /// <returns></returns>
        private bool CheckForAuthSuccess()
        {

            var htmlElements = webBrowserGTRF.Document.GetElementsByTagName("a").Cast<HtmlElement>()
                .Where(x => x.InnerText == "Вход");

            return htmlElements?.Count() == 0;

        }

        /// <summary>
        /// Устанавливает состояние элементов управления UI при успешной авторизации пользователя на сайте http://oed.gtrf.ru/
        /// </summary>
        private void SetAuthSuccessControls()
        {
            _userLoggedIn = true;
            captchaPictureBox.ImageLocation = "echo_logo.png";
            reloadCaptchaButton.Enabled = false;
            captchaTextBox.Text = string.Empty;
            captchaTextBox.Enabled = false;
            userNameTextBox.Enabled = false;
            userPasswordTextBox.Enabled = false;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private List<string> LoadMediaFilesList(string pathToFiles)
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

                foreach (var broadcast in _broadcastsToSend)
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

        private async Task GetDictionariesAsync()
        {
            _titles = await ExcelInteraction.GetDictionaryAsync(ExcelInteraction.ExcelFileName, "Titles");
            _anons = await ExcelInteraction.GetDictionaryAsync(ExcelInteraction.ExcelFileName, "Anons");
            _presenters = await ExcelInteraction.GetDictionaryAsync(ExcelInteraction.ExcelFileName, "Presenters");
            _directors = await ExcelInteraction.GetDictionaryAsync(ExcelInteraction.ExcelFileName, "Directors");
            _directorsSchedule = await ExcelInteraction.GetDirectorSheduleDictionaryAsync(ExcelInteraction.ExcelFileName, "DirectorsShedule");
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

        private string ParsePresenters(string presentersString)
        {
            string[] parsedString = presentersString.Split(new char[] { ',', ' ' });

            List<string> presentersList = new List<string>();

            foreach (string presenterName in parsedString)
            {
                if (_presenters.ContainsKey(presenterName.ToLower()))
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

            string resultString = _presenters[presentersArray[0].ToLower()];

            int i = 1;
            while (i < presentersArray.Length)
            {
                if (_presenters.ContainsKey(presentersArray[i].ToLower()))
                {
                    resultString += ", " + _presenters[presentersArray[i].ToLower()];
                }

                i++;
            }

            return resultString;
        }

        private string GetDirector(Broadcast bCast)
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
                    return _directors[_directorsSchedule[broadCast.Date].Interval1.ToLower()];
                }

                if (TimeSpan.Parse(broadCast.Time) > clock08 & TimeSpan.Parse(broadCast.Time) < clock15)
                {
                    return _directors[_directorsSchedule[broadCast.Date].Interval2.ToLower()];
                }

                if (TimeSpan.Parse(broadCast.Time) > clock15 & TimeSpan.Parse(broadCast.Time) < clock22)
                {
                    return _directors[_directorsSchedule[broadCast.Date].Interval3.ToLower()];
                }

                if (TimeSpan.Parse(broadCast.Time) > clock22 & TimeSpan.Parse(broadCast.Time) < clock23)
                {
                    return _directors[_directorsSchedule[broadCast.Date].Interval4.ToLower()];
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
