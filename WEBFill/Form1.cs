using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool userLoggedIn = false;

        public bool excelLoaded = false;

        public WebForm webForm;

        public bool NewPageLoaded = false;

        public string currentDir = Application.StartupPath + @"\mp3\";

        List<Broadcast> broadcastsToSend = new List<Broadcast>();

        string sourceHTMLText;

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
        }

        private void authButton_Click(object sender, EventArgs e)
        {

            webForm = new WebForm(webBrowserGTRF, captchaTextBox.Text);
            webForm.WebFormAuth(userNameTextBox.Text, userPasswordTextBox.Text);

            NewPageLoaded = false;
            while (NewPageLoaded == false)
            {
                Application.DoEvents();
            }
            NewPageLoaded = false;

            if (!CheckForAuthSuccess())
            {
                webBrowserGTRF.Navigate("http://oed.gtrf.ru/auth");

                NewPageLoaded = false;
                while (NewPageLoaded == false)
                {
                    Application.DoEvents();
                }
                NewPageLoaded = false;

                MessageBox.Show(
                    "Проверьте правильность заполнения данных аутентификации и правильность ввода кода с картинки", "Ошибка аутентификации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                captchaTextBox.Text = String.Empty;
                LoadCaptchaImage();
                return;
            }
;
            LoginNotify?.Invoke();

            if (excelLoaded == true)
            {
                //startSendingButton.Enabled = true; Не удалять!!!!
            }

        }

        private void loadFromExcelButton_Click(object sender, EventArgs e)
        {
            broadcastsToSend = ExcelInteraction.LoadListFromExcel(openExcelFileDialog);
            if (broadcastsToSend == null)
            {
                _ = MessageBox.Show("В открытом файле нет передач, готовых к выгрузке!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            excelLoaded = true;
            if (userLoggedIn == true)
            {
                //startSendingButton.Enabled = true; Не удалять!!!
            }
            createBroadcastTableButton.Enabled = true;
        }

        private void StartSendingButton_Click(object sender, EventArgs e)
        {

            startSendingButton.Enabled = false;
            loadFromExcelButton.Enabled = false;
            int i = 0;
            foreach (var broadcast in broadcastsToSend)
            {
                if (!broadcast.FileExists)
                {
                    continue;
                }

                labelBroadcastToSend.Text = "Передача: " + broadcast.Title + " от " + broadcast.DateAired;

                NewPageLoaded = false;
                webBrowserGTRF.Navigate("http://oed.gtrf.ru/materials/edit");
                while (NewPageLoaded == false)
                {
                    Application.DoEvents();
                }
                NewPageLoaded = false;

                webForm.FillWebForm(webBrowserGTRF, broadcast);
                while (NewPageLoaded == false)
                {
                    Application.DoEvents();
                }
                NewPageLoaded = false;

                webBrowserGTRF.Document?.GetElementById("fileUpload")?.Focus();
                SendKeys.Send(" " + currentDir + broadcast.FileNameFormat());
                SendKeys.SendWait(" \n");
                foreach (HtmlElement input in webBrowserGTRF.Document.GetElementsByTagName("button"))
                {
                    if (input.GetAttribute("InnerText") == " Загрузить файл")
                    {
                        input.InvokeMember("click");
                    }
                }
                while (NewPageLoaded == false)
                {
                    Application.DoEvents();
                }
                NewPageLoaded = false;

                foreach (HtmlElement input in webBrowserGTRF.Document.GetElementsByTagName("button"))
                {
                    if (input.GetAttribute("InnerText") == "Сохранить и отправить")
                    {
                        input.InvokeMember("click");
                    }
                }

                while (NewPageLoaded == false)
                {
                    Application.DoEvents();
                }
                NewPageLoaded = false;
                ExcelInteraction.SetTransmittedFlag(ExcelInteraction.ExcelFileName, broadcast);
                i++;
                labelTransmitCount.Text = "Передано " + i + " из " + broadcastsToSend.Count;

            }
            webBrowserGTRF.Navigate("http://oed.gtrf.ru/");
            _ = MessageBox.Show($"Передача файлов завершена\nВсего передано выпусков программ: {i}", "Завершено!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            labelBroadcastToSend.Text = $"Передано {i} выпусков программ";
        }


        private void PageLoaded(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            NewPageLoaded = true;
        }

        private void CreateBroadcastTableButton_Click(object sender, EventArgs e)
        {
            BroadcastDb broadcastTable = new BroadcastDb(ExcelInteraction.ExcelFileName, progressBar1);
            //createBroadcastTableButton.Enabled = false;
        }

        private void reloadCaptchaButton_Click(object sender, EventArgs e)
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

        private void userNameTextBox_TextChanged(object sender, EventArgs e)
        {
            SetAuthButtonState();
        }

        private void userPasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            SetAuthButtonState();
        }

        private void captchaTextBox_TextChanged(object sender, EventArgs e)
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

            while (NewPageLoaded == false)
            {
                Application.DoEvents();
            }
            NewPageLoaded = false;

            sourceHTMLText = webBrowserGTRF.DocumentText;
            var offset = sourceHTMLText.IndexOf("/captcha/");
            var s = sourceHTMLText.Substring(offset + 9, 36);
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
            userLoggedIn = true;
            captchaPictureBox.ImageLocation = "echo_logo.png";
            reloadCaptchaButton.Enabled = false;
            captchaTextBox.Text = string.Empty;
            captchaTextBox.Enabled = false;
            userNameTextBox.Enabled = false;
            userPasswordTextBox.Enabled = false;

        }
    }
}
