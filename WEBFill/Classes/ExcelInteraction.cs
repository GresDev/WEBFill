using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WEBFill.Classes
{
    public static class ExcelInteraction
    {
        private static string _excelFileName;

        /// <summary>
        /// Возвращает список листов в выбранном файле Excel
        /// </summary>
        /// <param name="schemaDataTable"></param>
        /// <returns></returns>
        private static List<string> GetSheetsName(DataTable schemaDataTable)
        {
            var sheets = new List<string>();
            foreach (var dataRow in schemaDataTable.AsEnumerable())
            {
                sheets.Add(dataRow.ItemArray[2].ToString());
            }
            return sheets;
        }


        /// <summary>
        /// Возвращает список передач, записанных в выбранном файле Excel
        /// </summary>
        /// <param name="openFileDialog"></param>
        /// <param name="selectedExcelFileName"></param>
        /// <returns></returns>
        public static async Task<List<Broadcast>> LoadListFromExcelAsync(OpenFileDialog openFileDialog, Label selectedExcelFileName)
        {
            List<Broadcast> broadcasts = new List<Broadcast>();
            string connectionString;
            string mp3Directory = @".\mp3\";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _excelFileName = openFileDialog.FileName;
            }
            else
            {
                return null;
            }

            switch (Path.GetExtension(_excelFileName))
            {
                case ".xlsx":
                case ".xls":
                    connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={_excelFileName}; Extended Properties=\"Excel 12.0 Xml; HDR=YES; IMEX=1;\"";
                    //connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0; Data Source={_excelFileName}; Extended Properties=\"Excel 8.0; HDR=YES; IMEX=1\"";
                    break;
                default:
                    MessageBox.Show("Выберите файл Excel!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
            }

            string excelQuery = "SELECT * FROM [Broadcasts$]";

            using (OleDbConnection excelConnection = new OleDbConnection(connectionString))
            {
                await excelConnection.OpenAsync();
                DataTable dtShema = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                List<string> sheetNames = ExcelInteraction.GetSheetsName(dtShema);

                if (!sheetNames.Contains("Broadcasts$"))
                {
                    MessageBox.Show("Выбранный файл не содержит лист \"Broadcasts\"\nc данными о передаваемых передачах!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                OleDbCommand excelCommand = new OleDbCommand(excelQuery, excelConnection);
                OleDbDataAdapter excelDataAdapter = new OleDbDataAdapter(excelCommand);

                DataTable sheetBroadcasts = new DataTable();
                excelDataAdapter.Fill(sheetBroadcasts);

                var broadcastsDataReader = sheetBroadcasts.CreateDataReader();

                if (broadcastsDataReader.HasRows)
                {
                    while (await broadcastsDataReader.ReadAsync())
                    {
                        object[] items = new object[broadcastsDataReader.FieldCount];
                        broadcastsDataReader.GetValues(items);

                        Broadcast broadcast = new Broadcast
                        {
                            Id = Convert.ToInt16(items[0]),
                            Title = Convert.ToString(items[1]),
                            DateAired = Convert.ToString(items[2]),
                            DateAiredEnd = Convert.ToString(items[3]),
                            Vendor = Convert.ToString(items[4]),
                            Author = Convert.ToString(items[5]),
                            Composer = Convert.ToString(items[6]),
                            Director = Convert.ToString(items[7]),
                            Fragments = Convert.ToString(items[8]),
                            Presenters = Convert.ToString(items[9]),
                            Guests = Convert.ToString(items[10]),
                            BroadcastCountryId = Convert.ToString(items[11]),
                            Languages = Convert.ToString(items[12]),
                            Anons = Convert.ToString(items[13]),
                            FileName = Convert.ToString(items[14]),
                            Transmitted = Convert.ToString(items[15]),
                            Sha256 = Convert.ToString(items[18]),
                        };
                        broadcast.FileExists = true;
                        if (!File.Exists(mp3Directory + broadcast.FileName))
                        {
                            MessageBox.Show($"Файл \"{broadcast.FileName}\" не найден!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            broadcast.FileExists = false;
                        }

                        broadcasts.Add(broadcast);
                    }
                }
            }

            selectedExcelFileName.Text = Path.GetFileName(_excelFileName);

            return broadcasts;
        }

        /// <summary>
        /// Возвращает словарь элементов, записаннх на указанном листе выбранного файла Excel
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> GetDictionaryAsync(string tableName)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={_excelFileName}; Extended Properties=\"Excel 12.0 Xml; HDR=YES; IMEX=1;\"";
            string excelQuery = $"SELECT * FROM [{tableName}$]";

            using (OleDbConnection excelConnection = new OleDbConnection(connectionString))
            {
                await excelConnection.OpenAsync();
                OleDbCommand excelCommand = new OleDbCommand(excelQuery, excelConnection);
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(excelCommand);
                DataTable excelSheet = new DataTable();
                dataAdapter.Fill(excelSheet);
                var sheetDataReader = excelSheet.CreateDataReader();

                while (await sheetDataReader.ReadAsync())
                {
                    dictionary.Add(Convert.ToString(sheetDataReader.GetValue(0)).ToLower(), Convert.ToString(sheetDataReader.GetValue(1)));
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Возвращает словарь, содержащий расписания звукорежиссеров
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, DirectorsSchedule>> GetDirectorScheduleDictionaryAsync(string tableName)
        {
            Dictionary<string, DirectorsSchedule> dictionary = new Dictionary<string, DirectorsSchedule>();
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={_excelFileName}; Extended Properties=\"Excel 12.0 Xml; HDR=YES; IMEX=1;\"";
            string excelQuery = $"SELECT * FROM [{tableName}$]";

            using (OleDbConnection excelConnection = new OleDbConnection(connectionString))
            {
                await excelConnection.OpenAsync();
                OleDbCommand excelCommand = new OleDbCommand(excelQuery, excelConnection);
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(excelCommand);
                DataTable excelSheet = new DataTable();
                dataAdapter.Fill(excelSheet);
                var sheetDataReader = excelSheet.CreateDataReader();

                while (await sheetDataReader.ReadAsync())
                {
                    DirectorsSchedule shedule = new DirectorsSchedule()
                    {
                        Interval1 = Convert.ToString(sheetDataReader.GetValue(2)).Trim(),
                        Interval2 = Convert.ToString(sheetDataReader.GetValue(3)).Trim(),
                        Interval3 = Convert.ToString(sheetDataReader.GetValue(4)).Trim(),
                        Interval4 = Convert.ToString(sheetDataReader.GetValue(5)).Trim(),
                    };

                    dictionary.Add(Convert.ToDateTime(sheetDataReader.GetValue(0)).ToString("dd/MM/yyyy"), shedule);
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Записывает данные передачи в выбранный файл Excel
        /// </summary>
        /// <param name="broadcast"></param>
        /// <returns></returns>
        public static async Task WriteBroadcastToTableAsync(Broadcast broadcast)
        {

            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={_excelFileName}; Extended Properties=\"Excel 12.0 Xml; HDR=YES; IMEX=3;\"";

            using (OleDbConnection excelConnection = new OleDbConnection(connectionString))
            {
                await excelConnection.OpenAsync();

                string excelQuery = $"INSERT INTO [Broadcasts$] (Id, Title, DateAired, DateAiredEnd, Vendor, Author, Composer, Director, Fragments, Presenters, Guests, BroadcastCountryId, Languages, Anons, FileName, Transmitted, [Date], [Time], SHA256) values ({broadcast.Id}, '{broadcast.Title}', '{broadcast.DateAired}', '{broadcast.DateAiredEnd}', '{broadcast.Vendor}', '{broadcast.Author}', '{broadcast.Composer}', '{broadcast.Director}', '{broadcast.Fragments}', '{broadcast.Presenters}', '{broadcast.Guests}', '{broadcast.BroadcastCountryId}', '{broadcast.Languages}', '{broadcast.Anons}', '{broadcast.FileName}', '{broadcast.Transmitted}', '{broadcast.Date}', '{broadcast.Time}', '{broadcast.Sha256}')";
                OleDbCommand excelCommand = new OleDbCommand(excelQuery, excelConnection);
                await excelCommand.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Устанавливает флаг "передано" в файле Excel для переданного выпуска программы
        /// </summary>
        /// <param name="broadcast"></param>
        public static void SetTransmittedFlag(Broadcast broadcast)
        {
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={_excelFileName}; Extended Properties=\"Excel 12.0 Xml; HDR=YES; IMEX=3;\"";

            using (OleDbConnection excelConnection = new OleDbConnection(connectionString))
            {
                excelConnection.Open();
                string excelQuery = $"UPDATE [Broadcasts$] SET Transmitted = '1' WHERE Id = '{broadcast.Id}'";
                OleDbCommand excelCommand = new OleDbCommand(excelQuery, excelConnection);
                excelCommand.ExecuteNonQuery();
            }
        }
    }
}
