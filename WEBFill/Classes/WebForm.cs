using System;
using System.Linq;
using System.Windows.Forms;

namespace WEBFill.Classes
{
    public static class WebForm
    {
        /// <summary>
        /// Аутентификация на сайте http://oed.gtrf.ru/
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="captcha"></param>
        public static void WebFormAuth(WebBrowser webBrowser, string login, string password, string captcha)
        {
            try
            {
                webBrowser.Document.GetElementById("login").InnerText = login;
                webBrowser.Document.GetElementById("password").Focus();
                webBrowser.Document.GetElementById("password").InnerText = password;
                webBrowser.Document.GetElementById("captcha-input").InnerText = captcha;

                foreach (HtmlElement input in webBrowser.Document.GetElementsByTagName("button"))
                {
                    if (input.GetAttribute("InnerText") == "Войти")
                    {
                        input.InvokeMember("click");
                    }
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Проверьте доступность сайта http://oed.gtrf.ru/ и попробуйте снова.", "Нет доступа к сайту http://oed.gtrf.ru/", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Заполнение web формы данными передаваемой передачи
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <param name="broadcast"></param>
        public static void FillWebForm(WebBrowser webBrowser, Broadcast broadcast)
        {

            if (webBrowser.Document != null)
            {
                webBrowser.Document.GetElementById("title").InnerText = broadcast.Title;
                webBrowser.Document.GetElementById("date_aired").InnerText = broadcast.DateAired;
                if (broadcast.DateAiredEnd != "-")
                    webBrowser.Document.GetElementById("date_air_end").InnerText = broadcast.DateAiredEnd;
                webBrowser.Document.GetElementById("vendor").InnerText = broadcast.Vendor;
                webBrowser.Document.GetElementById("author").InnerText = broadcast.Author;
                webBrowser.Document.GetElementById("composer").InnerText = broadcast.Composer;
                webBrowser.Document.GetElementById("director").InnerText = broadcast.Director;
                webBrowser.Document.GetElementById("fragments").InnerText = broadcast.Fragments;
                webBrowser.Document.GetElementById("presenters").InnerText = broadcast.Presenters;
                webBrowser.Document.GetElementById("guests").InnerText = broadcast.Guests;
                if (broadcast.BroadcastCountryId != "Россия")
                    webBrowser.Document.GetElementById("broadcast_country_id").InnerText = broadcast.BroadcastCountryId;
                if (broadcast.Languages != "Русский")
                    webBrowser.Document.GetElementById("languages").InnerText = broadcast.Languages;
                webBrowser.Document.GetElementById("anons").InnerText = broadcast.Anons;

                foreach (HtmlElement input in webBrowser.Document.GetElementsByTagName("button"))
                {
                    if (input.GetAttribute("InnerText") == "Сохранить")
                    {
                        input.InvokeMember("click");
                    }
                }
            }

            while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Возвращает true, если пользователь авторизован на сайте http://oed.gtrf.ru/, иначе false
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <returns></returns>
        public static bool CheckForAuthSuccess(WebBrowser webBrowser)
        {

            var htmlElements = webBrowser.Document.GetElementsByTagName("a").Cast<HtmlElement>()
                .Where(x => x.InnerText == "Вход");

            return htmlElements?.Count() == 0;

        }

        /// <summary>
        /// Загрузка изображения CAPTCHA
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <param name="pictureBox"></param>
        /// <param name="waitForPageCompleted"></param>
        public static void LoadCaptchaImage(WebBrowser webBrowser, PictureBox pictureBox, Action waitForPageCompleted)
        {
            webBrowser.Url = new Uri("http://oed.gtrf.ru/auth");

            waitForPageCompleted?.Invoke();

            var _sourceHtmlText = webBrowser.DocumentText;
            var offset = _sourceHtmlText.IndexOf("/captcha/", StringComparison.Ordinal);
            var s = _sourceHtmlText.Substring(offset + 9, 36);
            pictureBox.ImageLocation = "http://oed.gtrf.ru/captcha/" + s;
        }

    }
}
