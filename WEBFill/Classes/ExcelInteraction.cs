﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;

namespace WEBFill.Classes
{
    public static class ExcelInteraction
    {
        public static string ExcelFileName { get; set; }

        public static List<string> GetSheetsName(DataTable schemaDataTable)
        {
            var sheets = new List<string>();
            foreach (var dataRow in schemaDataTable.AsEnumerable())
            {
                sheets.Add(dataRow.ItemArray[2].ToString());
            }
            return sheets;
        }

        public static List<Broadcast> LoadListFromExcel(OpenFileDialog openFileDialog, Label selectedExcelFileName)
        {
            List<Broadcast> broadcasts = new List<Broadcast>();
            string connectionString;
            string mp3Directory = @".\mp3\";

            //openFileDialog.FileName = "";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExcelFileName = openFileDialog.FileName;
            }
            else
            {
                return null;
            }

            switch (Path.GetExtension(ExcelFileName))
            {
                case ".xlsx":
                case ".xls":
                    connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={ExcelFileName}; Extended Properties=\"Excel 12.0 Xml; HDR=YES; IMEX=1;\"";
                    //connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0; Data Source={excelFileName}; Extended Properties=\"Excel 8.0; HDR=YES; IMEX=1\"";
                    break;
                default:
                    MessageBox.Show("Выберите файл Excel!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
            }

            string excelQuery = "SELECT * FROM [Broadcasts$]";

            using (OleDbConnection excelConnection = new OleDbConnection(connectionString))
            {
                excelConnection.Open();
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

                //broadcasts.Clear();

                if (broadcastsDataReader.HasRows)
                {
                    while (broadcastsDataReader.Read())
                    {
                        object[] items = new object[broadcastsDataReader.FieldCount];
                        broadcastsDataReader.GetValues(items);
                        //if (Convert.ToString(items[15]) == "0")
                        //{
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
                        };
                        broadcast.FileExists = true;
                        if (!File.Exists(mp3Directory + broadcast.FileName))
                        {
                            MessageBox.Show($"Файл \"{broadcast.FileName}\" не найден!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            broadcast.FileExists = false;
                        }

                        broadcasts.Add(broadcast);
                        //}
                    }
                }
            }

            selectedExcelFileName.Text = Path.GetFileName(ExcelFileName);

            return broadcasts;
        }

        public static Dictionary<string, string> GetDictionary(string broadcastFileName, string tableName)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={broadcastFileName}; Extended Properties=\"Excel 12.0 Xml; HDR=YES; IMEX=1;\"";
            string excelQuery = $"SELECT * FROM [{tableName}$]";
            //string buffer;
            using (OleDbConnection excelConnection = new OleDbConnection(connectionString))
            {
                excelConnection.Open();
                OleDbCommand excelCommand = new OleDbCommand(excelQuery, excelConnection);
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(excelCommand);
                DataTable excelSheet = new DataTable();
                dataAdapter.Fill(excelSheet);
                var sheetDataReader = excelSheet.CreateDataReader();

                while (sheetDataReader.Read())
                {
                    //buffer = Convert.ToString(sheetDataReader.GetValue(0)).ToLower();
                    dictionary.Add(Convert.ToString(sheetDataReader.GetValue(0)).ToLower(), Convert.ToString(sheetDataReader.GetValue(1)));
                }
            }

            return dictionary;
        }

        public static Dictionary<string, DirectorsShedule> GetDirectorSheduleDictionary(string broadcastFileName, string tableName)
        {
            Dictionary<string, DirectorsShedule> dictionary = new Dictionary<string, DirectorsShedule>();
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={broadcastFileName}; Extended Properties=\"Excel 12.0 Xml; HDR=YES; IMEX=1;\"";
            string excelQuery = $"SELECT * FROM [{tableName}$]";
            //string buffer;
            using (OleDbConnection excelConnection = new OleDbConnection(connectionString))
            {
                excelConnection.Open();
                OleDbCommand excelCommand = new OleDbCommand(excelQuery, excelConnection);
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(excelCommand);
                DataTable excelSheet = new DataTable();
                dataAdapter.Fill(excelSheet);
                var sheetDataReader = excelSheet.CreateDataReader();

                while (sheetDataReader.Read())
                {
                    //buffer = Convert.ToString(sheetDataReader.GetValue(0)).ToLower();
                    DirectorsShedule shedule = new DirectorsShedule()
                    {
                        Interval1 = Convert.ToString(sheetDataReader.GetValue(2)).Trim(),
                        Interval2 = Convert.ToString(sheetDataReader.GetValue(3)).Trim(),
                        Interval3 = Convert.ToString(sheetDataReader.GetValue(4)).Trim(),
                        Interval4 = Convert.ToString(sheetDataReader.GetValue(5)).Trim(),
                    };
                    //dictionary.Add(Convert.ToString(sheetDataReader.GetValue(0)), shedule);
                    dictionary.Add(Convert.ToDateTime(sheetDataReader.GetValue(0)).ToString("dd/MM/yyyy"), shedule);
                }
            }

            return dictionary;
        }

        public static void BroadcastTableFill(string excelFileName, List<Broadcast> broadcastList, ProgressBar progressBar)
        {

            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={excelFileName}; Extended Properties=\"Excel 12.0 Xml; HDR=YES; IMEX=3;\"";
            string excelQuery;

            using (OleDbConnection excelConnection = new OleDbConnection(connectionString))
            {
                excelConnection.Open();

                int i = 0;

                foreach (Broadcast broadcast in broadcastList)
                {
                    excelQuery = $"INSERT INTO [Broadcasts$] (Id, Title, DateAired, DateAiredEnd, Vendor, Author, Composer, Director, Fragments, Presenters, Guests, BroadcastCountryId, Languages, Anons, FileName, Transmitted, [Date], [Time], SHA256) values ({broadcast.Id}, '{broadcast.Title}', '{broadcast.DateAired}', '{broadcast.DateAiredEnd}', '{broadcast.Vendor}', '{broadcast.Author}', '{broadcast.Composer}', '{broadcast.Director}', '{broadcast.Fragments}', '{broadcast.Presenters}', '{broadcast.Guests}', '{broadcast.BroadcastCountryId}', '{broadcast.Languages}', '{broadcast.Anons}', '{broadcast.FileName}', '{broadcast.Transmitted}', '{broadcast.Date}', '{broadcast.Time}', '{broadcast.Sha256}')";
                    OleDbCommand excelCommand = new OleDbCommand(excelQuery, excelConnection);
                    excelCommand.ExecuteNonQuery();
                    i++;
                    progressBar.Value = (i * 100) / broadcastList.Count;
                }
            }
        }

        public static List<BroadcastAuthor> GetAuthors(string excelFileName)
        {
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={excelFileName}; Extended Properties=\"Excel 12.0 Xml; HDR=YES; IMEX=3;\"";
            string excelQuery;
            List<BroadcastAuthor> broadcastAuthors = new List<BroadcastAuthor>();

            using (OleDbConnection excelConnection = new OleDbConnection(connectionString))
            {
                excelConnection.Open();
                excelQuery = "SELECT Id, Author FROM [Broadcasts$]";
                OleDbCommand excelCommand = new OleDbCommand(excelQuery, excelConnection);
                OleDbDataAdapter excelDataAdapter = new OleDbDataAdapter(excelCommand);

                DataTable sheetAuthors = new DataTable();
                excelDataAdapter.Fill(sheetAuthors);

                var authorsDataReader = sheetAuthors.CreateDataReader();

                while (authorsDataReader.Read())
                {
                    BroadcastAuthor bAuthor = new BroadcastAuthor
                    {
                        Id = Convert.ToInt32(authorsDataReader.GetValue(0)),
                        NameString = Convert.ToString(authorsDataReader.GetValue(1))
                    };
                    broadcastAuthors.Add(bAuthor);
                }
            }

            return broadcastAuthors;
        }

        public static void UpdateAuthors(string excelFileName, List<BroadcastAuthor> broadcastAutorsList)
        {
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={excelFileName}; Extended Properties=\"Excel 12.0 Xml; HDR=YES; IMEX=3;\"";
            string excelQuery;

            using (OleDbConnection excelConnection = new OleDbConnection(connectionString))
            {
                excelConnection.Open();

                foreach (BroadcastAuthor bAuthor in broadcastAutorsList)
                {
                    excelQuery = $"UPDATE [Broadcasts$] SET Author = '{bAuthor.NameString}', Presenters = '{bAuthor.NameString}' WHERE Id = '{bAuthor.Id}'";
                    OleDbCommand excelCommand = new OleDbCommand(excelQuery, excelConnection);
                    try
                    {
                        excelCommand.ExecuteNonQuery();
                    }
                    catch (OleDbException)
                    {
                        MessageBox.Show($"Ошибка при обновлении авторов в строке {bAuthor.Id}");
                        Environment.Exit(-1);
                    }

                }

            }
        }

        public static void SetTransmittedFlag(string excelFileName, Broadcast broadcast)
        {
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={excelFileName}; Extended Properties=\"Excel 12.0 Xml; HDR=YES; IMEX=3;\"";
            string excelQuery;

            using (OleDbConnection excelConnection = new OleDbConnection(connectionString))
            {
                excelConnection.Open();
                excelQuery = $"UPDATE [Broadcasts$] SET Transmitted = '1' WHERE Id = '{broadcast.Id}'";
                OleDbCommand excelCommand = new OleDbCommand(excelQuery, excelConnection);
                excelCommand.ExecuteNonQuery();
            }
        }
    }
}
