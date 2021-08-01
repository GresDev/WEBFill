using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WEBFill.Classes;

namespace WEBFill
{
    public partial class Form1 : Form
    {

        private const string CurrentYear = "2021";

        private bool _userLoggedIn;

        private bool _excelLoaded;

        private bool _newPageLoaded;

        private readonly string _currentDir = Application.StartupPath + @"\mp3\";

        private List<string> _mediaFilesList;

        private List<Broadcast> _broadcastsToSend = new List<Broadcast>();

        public Form1()
        {
            InitializeComponent();
            webBrowserGTRF.DocumentCompleted += OnPageLoaded;
            webBrowserGTRF.ScriptErrorsSuppressed = true;

            WebForm.LoadCaptchaImage(webBrowserGTRF, captchaPictureBox, WaitForPageCompleted);

            if (WebForm.CheckForAuthSuccess(webBrowserGTRF))
            {
                SetAuthSuccessControls();
            }

        }

        private void AuthButton_Click(object sender, EventArgs e)
        {

            WebForm.WebFormAuth(webBrowserGTRF, userNameTextBox.Text, userPasswordTextBox.Text, captchaTextBox.Text);

            WaitForPageCompleted();

            if (!WebForm.CheckForAuthSuccess(webBrowserGTRF))
            {
                webBrowserGTRF.Navigate("http://oed.gtrf.ru/auth");

                WaitForPageCompleted();

                MessageBox.Show(
                    "Проверьте правильность заполнения данных аутентификации и правильность ввода кода с картинки", "Ошибка аутентификации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                captchaTextBox.Text = String.Empty;

                WebForm.LoadCaptchaImage(webBrowserGTRF, captchaPictureBox, WaitForPageCompleted);

                return;
            }

            SetAuthSuccessControls();

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

            await Utils.GetDictionariesAsync();

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

            richTextBox1.Clear();

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


                webBrowserGTRF.Navigate("http://oed.gtrf.ru/materials/edit");
                WaitForPageCompleted();

                WebForm.FillWebForm(webBrowserGTRF, broadcast);
                WaitForPageCompleted();

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
                WaitForPageCompleted();

                foreach (HtmlElement input in webBrowserGTRF.Document.GetElementsByTagName("button"))
                {
                    if (input.GetAttribute("InnerText") == "Сохранить и отправить")
                    {
                        input.InvokeMember("click");
                    }
                }

                WaitForPageCompleted();

                ExcelInteraction.SetTransmittedFlag(broadcast);

                try
                {
                    richTextBox1.Text += broadcast.Id + ": " + broadcast.Title + " (" + broadcast.DateAired + ") - передан\n";
                    richTextBox1.Select(richTextBox1.Text.Length, 0);
                    richTextBox1.ScrollToCaret();
                }
                catch (ObjectDisposedException)
                {

                }

                i++;

                broadcastTransmittedNumberLabel.Text =
                    (int.Parse(broadcastTransmittedNumberLabel.Text) + 1).ToString();

                progressBar.Value = i * 100 / _broadcastsToSend.Count();




            }
            webBrowserGTRF.Navigate("http://oed.gtrf.ru/");
            MessageBox.Show($"Передача файлов завершена\nВсего передано выпусков программ: {i}", "Завершено!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Устанавливает признак окончания загрузки web страницы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageLoaded(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            _newPageLoaded = true;
        }


        /// <summary>
        /// Ожидание загрузки web страницы
        /// </summary>
        private void WaitForPageCompleted()
        {
            _newPageLoaded = false;
            while (_newPageLoaded == false)
            {
                Application.DoEvents();
            }

            _newPageLoaded = false;
        }

        private async void CreateBroadcastTableButton_Click(object sender, EventArgs e)
        {

            loadFromExcelButton.Enabled = false;

            startSendingButton.Enabled = false;

            createBroadcastTableButton.Enabled = false;

            _mediaFilesList = Utils.LoadMediaFilesList(_currentDir, _broadcastsToSend);

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
                string[] mediaInfo = Utils.ParseFileName(fileName, CurrentYear);

                mediaInfo[5] = Utils.ParsePresenters(fileName, mediaInfo[5]);

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
                    broadcast.Title = Utils.Titles[mediaInfo[1].ToLower()];
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

                broadcast.Anons = Utils.Anons[broadcast.Title.ToLower()];
                if (broadcast.DateAiredEnd == "-")
                {
                    broadcast.Fragments = "В прилагаемом файле выпуск программы от " + broadcast.Date + ".";
                }
                else
                {
                    broadcast.Fragments = "В прилагаемом файле выпуски программы, вышедшие с " + mediaInfo[2] + " по " +
                                          mediaInfo[4];
                }

                broadcast.Director = Utils.GetDirector(broadcast, Utils.Directors, Utils.DirectorsSchedule);

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

            if (_broadcastsToSend.Any() && _userLoggedIn)
            {
                startSendingButton.Enabled = true;
            }

            loadFromExcelButton.Enabled = true;
        }

        private void ReloadCaptchaButton_Click(object sender, EventArgs e)
        {
            captchaTextBox.Text = String.Empty;
            WebForm.LoadCaptchaImage(webBrowserGTRF, captchaPictureBox, WaitForPageCompleted);
        }

        /// <summary>
        /// Установка состояний кнопки аутентификации на сайте http://oed.gtrf.ru/ в зависимости от заполнения полей с учетными данными и поля captcha
        /// </summary>
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
        /// Установка состояний элементов управления UI при успешной авторизации пользователя на сайте http://oed.gtrf.ru/
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

    }
}
