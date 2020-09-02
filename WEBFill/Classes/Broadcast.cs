using System.Text.RegularExpressions;

namespace WEBFill.Classes
{
    public class Broadcast
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string DateAired { get; set; }
        public string DateAiredEnd { get; set; }
        public string Vendor { get; set; }
        public string Author { get; set; }
        public string Composer { get; set; }
        public string Director { get; set; }
        public string Fragments { get; set; }
        public string Presenters { get; set; }
        public string Guests { get; set; }
        public string BroadcastCountryId { get; set; }
        public string Languages { get; set; }
        public string Anons { get; set; }
        public string FileName { get; set; }
        public string Transmitted { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public bool FileExists { get; set; }

        public Broadcast()
        {
            this.DateAiredEnd = "-";
            this.Vendor = "-";
            this.Composer = "-";
            this.Fragments = "-";
            this.Guests = "-";
            this.BroadcastCountryId = "Россия";
            this.Languages = "Русский";
            this.Transmitted = "0";
        }

        public string FileNameFormat()
        {
            string fileName = this.FileName;
            fileName = Regex.Replace(fileName, "[+^%~()]", "{$0}");
            return fileName;
        }

    }
}
