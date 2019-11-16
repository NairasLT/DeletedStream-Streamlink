using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Live
{
    class Config
    {
        Other log = new Other();
        Checker ch = new Checker();
        public int hlsinterval;
        public void configURL()
        {
            log.FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\LiveConfig";
            log.URLpath = log.FolderPath + "\\configURL.txt";
            log.INTpath = log.FolderPath + "\\configINTERVAL.txt";
            log.MainPath = log.FolderPath + "\\Main.txt";
            if (log.ConfigCheckExist(log.FolderPath) == false)
            {
                Console.WriteLine("The Config Folder Dosent Exist, Creating it");
                Directory.CreateDirectory(log.FolderPath);

                if (log.ConfigCheckExist(log.URLpath) == false)
            {
                // FIRST 
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

            if(log.ConfigCheckExist(log.INTpath) == false)
                {
                    configINT();
                }

            if(log.ConfigCheckExist(log.MainPath) == false)
                {
                        Thread.Sleep(1000);
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Do you want to Enable Auto uploading to youtube?\n");
                        Console.WriteLine("If yes to get client_secrets.json go to http://console.developers.google.com/ \n   create a project -> create crendentials -> Oauth2 -> download json rename it to 'client_secrets.json'\n   and move the file to 'C:\\client_secrets.json'");
                        Console.WriteLine("NOTE: if you are auto-uploading to youtube there is a QUOTA limit 5-6 videos every day.\n   The Quota resets at PT Midnight");
                        Console.ResetColor();
                        Console.WriteLine("\n\nTo choose type, yes or no, in lowercase");
                        Thread.Sleep(1000);
                        string inp1 = Console.ReadLine();

                        if (inp1.Contains("yes"))
                        {
                            ch.AutoUploading = true;
                            Console.WriteLine("Auto-Uploading Enabled.");
                            Thread.Sleep(1300);
                        }
                        if (inp1.Contains("no"))
                        {
                            ch.AutoUploading = false;
                            Console.WriteLine("Auto-Uploading Disabled.");
                            Thread.Sleep(1300);
                        }

                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("USELESS: Streamlink HLS timeout in SECONDS. default is 1800Sec - 30 mins");
                        Console.WriteLine("Type a higher number if your internet is slow, if not type, 60");
                        Console.ResetColor();
                        hlsinterval = int.Parse(Console.ReadLine());
                        using (StreamWriter sw = File.CreateText(log.MainPath))
                        {
                            sw.Write(ch.AutoUploading.ToString() + "\n" + hlsinterval);
                        }
                        Thread.Sleep(1000);

                    }

                }

            }
        }

        public void configINT()
        {
            Console.Clear();
            Console.WriteLine("The URL checking Intervals are not Setup, please enter the intervals.\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("For YouTube Minimum Milliseconds '360000' if lower Streamlink will download the same live\nstream again!\n\n");
            Console.ResetColor();
            Thread.Sleep(2000);
            int lineCount = log.CountLines(log.URLpath);


            string[] ints = new string[lineCount + 1]; // Nes esa 0 based jai bus error patikrink!! <---
            for (var i = 1; i <= lineCount; i++)
            {
                local();

                void local()
                {
                    Console.WriteLine(string.Format("Please enter The '{0}' URL Interval in MILLISECONDS to Check", i)); // REIK PAKEIST PASKUI KAD RODYTU URL! NE NEREIK KOLKAS
                    int temp = int.Parse(Console.ReadLine());
                    if (temp < 360000)
                    {
                        Console.WriteLine("Enter a value in MILLISECONDS higher than 360000: ");
                        local();
                    }
                    else
                    {
                        ints[i] = temp.ToString();
                    }
                }

            }
            using (StreamWriter sw = File.CreateText(log.INTpath))
            {
                    sw.Write(string.Join("\n", ints).Trim());
            }



            //Console.WriteLine(string.Join("\n", urls));
            Thread.Sleep(1000);
            Console.Clear();
            Console.WriteLine("Setup is Done.\n" + string.Join("\n", ints));
        }
        int urllines;
        int intlines;
        string URLConf;
        string INTConf;

        public void configRead()
        {
            try
            {
                URLConf = System.IO.File.ReadAllText(log.URLpath);
                INTConf = System.IO.File.ReadAllText(log.INTpath);
                string mainpathtext = File.ReadAllText(log.MainPath);
                hlsinterval = int.Parse(GetLine(mainpathtext, 2));
                ch.AutoUploading = bool.Parse(GetLine(mainpathtext, 1));
                Console.WriteLine("=================\n\n Info from config file\n");
                Console.WriteLine("Auto Uploading: " + ch.AutoUploading);
                Console.WriteLine("HLS Timeout: " + hlsinterval);
                Console.WriteLine(URLConf);
                Console.WriteLine(INTConf);
                urllines = log.CountLines(log.URLpath);
                intlines = log.CountLines(log.INTpath);
                Console.WriteLine(string.Format("URL LINES: {0}\nINTERVAL LINES: {1}", urllines, intlines));
                Console.WriteLine("\n=================\n");
                Thread.Sleep(500);
                LoadConfig();
            }
            catch(Exception ex)
            {
                Console.WriteLine("If you see this message, please delete the LiveConfig Folder on your Desktop, this error means that URL and INTERVAL txt file dosent exist. " + ex.Message);
                Thread.Sleep(10000);
                Console.WriteLine("Close this window.");
            }
            
        }

        public void LoadConfig()
        {
            for (int i = 1; i <= urllines; i++)
            {
                string cont = GetLine(INTConf, i);
                Threader(GetLine(URLConf, i), int.Parse(cont), hlsinterval);
            }

        }

        public void Threader(string URL, int Delay, int HlsTimeout)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;


                while (true)
                {
                    ch.check(URL, HlsTimeout);
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
