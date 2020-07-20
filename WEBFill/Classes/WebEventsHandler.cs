using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WEBFill.Classes
{
    public class WebEventsHandler
    {

        static public void WebPageCompletedHandler(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //var _webBrowser = sender as WebBrowser;
            //string title = "";
            //switch (_webBrowser.Document.Title)
            //{
            //    case "Новый — Материалы — ОЭД":
            //        _ = MessageBox.Show("Страница для заполнения карточки передачи загружена!", "WebBrowser", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        break;
            //    case "Материалы — ОЭД":
            //        _ = MessageBox.Show("Страница со списком материалов загружена!", "WebBrowser", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        break;
            //    default:
            //        _ = MessageBox.Show("Страница\n" + _webBrowser.Document.Title + "\nзагружена!", "WebBrowser", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        break;
            //}
        }

    }
}
