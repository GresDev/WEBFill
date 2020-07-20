using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WEBFill.Classes;

namespace WEBFill
{
    public partial class Form1 : Form
    {

        public int count = 0;

        public bool loggedIn = false;

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
            pictureBox1.ImageLocation = "http://oed.gtrf.ru/captcha/" + s;

        }

        private void authButton_Click(object sender, EventArgs e)
        {
            if (captchaTextBox.Text != "")
            {
                webForm = new WebForm(webBrowserGTRF, captchaTextBox.Text);
                webForm.WebFormAuth();
                loggedIn = true;
                if (excelLoaded == true)
                {
                    startSendingButton.Enabled = true;
                }
                //authButton.Enabled = false;
            }
            else
            {
                _ = MessageBox.Show("Поле \"Captcha\" не заполнено!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                captchaTextBox.Focus();
            }

            pictureBox1.ImageLocation = "echo_logo.png";
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
            if (loggedIn == true)
            {
                startSendingButton.Enabled = true;
            }
            createBroadcastTableButton.Enabled = true;
            setAuthorsButton.Enabled = true;
            startSendingButton.Enabled = true;
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
                
                webBrowserGTRF.Document.GetElementById("fileUpload").Focus();
                //SendKeys.Send(" " + currentDir + broadcast.FileName);
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
            BroadcastDB broadcastTable = new BroadcastDB(ExcelInteraction.ExcelFileName, progressBar1);
            //createBroadcastTableButton.Enabled = false;
            //setAuthorsButton.Enabled = true;
        }

        private void SetAuthorsButton_Click(object sender, EventArgs e)
        {
            BroadcastDB broadcastTable = new BroadcastDB(ExcelInteraction.ExcelFileName);
        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}
