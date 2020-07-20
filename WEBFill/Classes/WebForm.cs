using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WEBFill.Classes
{
    public class WebForm
    {
        public WebBrowser WebBrowser { get; set; }
        public string Captcha { get; set; }
        public WebForm() { }
        public WebForm(WebBrowser webBrowser, string captha)
        {
            WebBrowser = webBrowser;
            Captcha = captha;
        }

        public void WebFormAuth()
        {
            WebBrowser.Document.GetElementById("login").InnerText = "echomsk";
            WebBrowser.Document.GetElementById("password").Focus();
            WebBrowser.Document.GetElementById("password").InnerText = "iot77twqc5";
            WebBrowser.Document.GetElementById("captcha-input").InnerText = Captcha;

            foreach (HtmlElement input in WebBrowser.Document.GetElementsByTagName("button"))
            {
                if (input.GetAttribute("InnerText") == "Войти")
                {
                    input.InvokeMember("click");
                }
            }

            //WebBrowser.DocumentCompleted += AuthPageCompleted;
        }

        //private void AuthPageCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    var _webBrowser = sender as WebBrowser;
        //    _webBrowser.DocumentCompleted -= AuthPageCompleted;
        //    _webBrowser.Navigate("http://oed.gtrf.ru/materials/edit");
        //}

        public void SendBroadcast(WebBrowser webbrowser)
        {

        }




        public void FillWebForm(WebBrowser webbrowser, Broadcast broadcast)
        {
            webbrowser.Document.GetElementById("title").InnerText = broadcast.Title;
            webbrowser.Document.GetElementById("date_aired").InnerText = broadcast.DateAired;
            if (broadcast.DateAiredEnd != "-") webbrowser.Document.GetElementById("date_air_end").InnerText = broadcast.DateAiredEnd;
            webbrowser.Document.GetElementById("vendor").InnerText = broadcast.Vendor;
            webbrowser.Document.GetElementById("author").InnerText = broadcast.Author;
            webbrowser.Document.GetElementById("composer").InnerText = broadcast.Composer;
            webbrowser.Document.GetElementById("director").InnerText = broadcast.Director;
            webbrowser.Document.GetElementById("fragments").InnerText = broadcast.Fragments;
            webbrowser.Document.GetElementById("presenters").InnerText = broadcast.Presenters;
            webbrowser.Document.GetElementById("guests").InnerText = broadcast.Guests;
            if (broadcast.BroadcastCountryId != "Россия") webbrowser.Document.GetElementById("broadcast_country_id").InnerText = broadcast.BroadcastCountryId;
            if (broadcast.Languages != "Русский") webbrowser.Document.GetElementById("languages").InnerText = broadcast.Languages;
            webbrowser.Document.GetElementById("anons").InnerText = broadcast.Anons;

            foreach (HtmlElement input in webbrowser.Document.GetElementsByTagName("button"))
            {
                if (input.GetAttribute("InnerText") == "Сохранить")
                {
                    input.InvokeMember("click");
                }
            }

            while (webbrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
        }




    }
}
