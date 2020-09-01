using System.Linq;
using System.Windows.Forms;

namespace WEBFill.Classes
{
    public class WebForm
    {
        public WebBrowser WebBrowser { get; set; }
        public string Captcha { get; set; }
        public WebForm(WebBrowser webBrowser, string captcha)
        {
            WebBrowser = webBrowser;
            Captcha = captcha;
        }

        public void WebFormAuth(string login, string password)
        {
            WebBrowser.Document.GetElementById("login").InnerText = login;
            WebBrowser.Document.GetElementById("password").Focus();
            WebBrowser.Document.GetElementById("password").InnerText = password;
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
