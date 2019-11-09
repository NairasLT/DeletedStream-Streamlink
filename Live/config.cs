using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Live
{
    class Config
    {
        Other log = new Other();
        Checker ch = new Checker();
        public void configURL()
        {


            if (log.ConfigCheckExist(log.URLpath) == false)
            {
                // FIRST 
                Console.WriteLine("The Config File Dosent Exist, Running the Setup!");
                Thread.Sleep(1000);
                Console.WriteLine("Please Enter how many URLS you want to check!");
                int AmountCheck = int.Parse(Console.ReadLine());
                string[] urls = new string[AmountCheck + 1]; // Nes esa 0 based
                for (var i = 1; i <= AmountCheck; i++)
                {
                    Console.WriteLine(string.Format("Please enter The '{0}' URL to Check", i));
                    urls[i] = Console.ReadLine();
                }
                using (StreamWriter sw = File.CreateText(log.URLpath))
                {
                    sw.Write(string.Join("\n", urls).Trim());
                }
                Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine("Setup is Done For Urls.\n"+string.Join("\n", urls));
                Thread.Sleep(3000);

            if(log.ConfigCheckExist(log.INTpath) == false)
                {
                    configINT();
                }
            }
        }

        public void configINT()
        {
            Console.Clear();
            Console.WriteLine("The URL checking Intervals are not Setup, please enter the intervals.");
            Thread.Sleep(1000);
            int lineCount = log.CountLines(log.URLpath);

            string[] ints = new string[lineCount + 1]; // Nes esa 0 based jai bus error patikrink!! <---
            for (var i = 1; i <= lineCount; i++)
            {
                Console.WriteLine(string.Format("Please enter The '{0}' URL Interval in MILLISECONDS to Check", i)); // REIK PAKEIST PASKUI KAD RODYTU URL! NE NEREIK KOLKAS
                ints[i] = Console.ReadLine();
            }
            using (StreamWriter sw = File.CreateText(log.INTpath))
            {
                    sw.Write(string.Join("\n", ints).Trim());
            }
            //Console.WriteLine(string.Join("\n", urls));
            Thread.Sleep(1000);
            Console.Clear();
            Console.WriteLine("Setup is Done.\n" + string.Join("\n", ints));
            Thread.Sleep(1000);
        }
        int urllines;
        int intlines;
        string URLConf;
        string INTConf;

        public void configRead()
        {
            URLConf = System.IO.File.ReadAllText(log.URLpath);
            INTConf = System.IO.File.ReadAllText(log.INTpath);
            Console.WriteLine(URLConf);
            Console.WriteLine(INTConf);
            urllines = log.CountLines(log.URLpath);
            intlines = log.CountLines(log.INTpath);
            Console.WriteLine(string.Format("URL LINES: {0}\nINTERVAL LINES: {1}", urllines, intlines));
            Thread.Sleep(500);
            LoadConfig();
        }

        public void LoadConfig()
        {
            for (int i = 1; i <= urllines; i++)
            {
                string cont = GetLine(INTConf, i);
                Threader(GetLine(URLConf, i), int.Parse(cont));
            }

        }

        public void Threader(string URL, int Delay)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    ch.check(URL);
                    System.Threading.Thread.Sleep(Delay);
                }
            }).Start();
        }

        string GetLine(string text, int lineNo)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }

    }
}
