using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Json;
using Newtonsoft.Json;
using System.Collections;
using Mono.Data;
using Mono.Data.Sqlite;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

 namespace TestApp
{

    public class _database
    {
        string url = "http://webservices.educom.nu/services/first/";
        int nummer = 1;

        // context definieren
        private Context context;

        //database maken
        public void createDatabase()
        {
            Resources res = this.context.Resources;
            string app_name = res.GetString(Resource.String.app_name);
            string app_version = res.GetString(Resource.String.app_version);
            string createTableData = res.GetString(Resource.String.createTableData);
            Console.WriteLine(createTableData);

            string dbname = "_db_" + app_name + "_" + app_version + ".sqlite";
            Console.WriteLine(dbname);
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string pathToDatabase = Path.Combine(documentsPath, dbname);
            Console.WriteLine(pathToDatabase);

            if (!File.Exists(pathToDatabase))
            {
                SqliteConnection.CreateFile(pathToDatabase);
                var connectionString = String.Format("Data Source={0}; Version = 3;", pathToDatabase);
                using (var conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        //table data
                        cmd.CommandText = createTableData;
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            Console.WriteLine("einde create database");
        }

        private void DownloadData()
        {
            Console.WriteLine("DownloadData gestart");
            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;

                byte[] myDataBuffer = webClient.DownloadData(this.url);
            try
            {
                string download = Encoding.ASCII.GetString(myDataBuffer);
                JsonValue value = JsonValue.Parse(download);

                foreach (JsonObject result in value)
                {
                    _database.dataRecord record = new dataRecord(result);
                    Console.WriteLine("code: " + record.code + ", description: " + record.description + ", id: " + record.id);
                 //   Console.WriteLine(result["code"] + "=" + result["description"]);
                }
            }

            catch (WebException)
            {
                //niks
            }
        }

        public class dataRecord
        {
            public int id;
            public string code;
            public string description;

            public dataRecord(JsonValue record)
            {
                this.code = (string)record["code"];
                this.description = (string)record["description"];
                this.id = _database.nummer;
                _database.nummer++;
            }
        }

        //constructor
        public _database (Context context)
        {
            this.context = context;
            this.createDatabase();
            this.DownloadData();
        }
    }
}