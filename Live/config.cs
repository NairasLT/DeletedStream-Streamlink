using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Threading;

namespace Live
{
    class Config
    {
        Other log = new Other();
        Checker ch = new Checker();
        public int hlsinterval;
        public bool Global_EmailNotifications;
        bool tempSetupUploading;
        bool tempSetupNotifications;
        string tempEmail;
        string tempPass;
        string tempSendTo;
        public void configURL()
        {

            if (log.ConfigCheckExist(log.FolderPath) == false)
            {
                Console.WriteLine("The Config Folder Dosent Exist, Creating it");
                Directory.CreateDirectory(log.FolderPath);

                if (log.ConfigCheckExist(log.URLpath) == false)
              {
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
                    sw.Close();
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
                        Console.WriteLine("\nTo choose type, yes or no, in lowercase");
                        string inp1 = Console.ReadLine();
                        if (inp1.Contains("yes"))
                        {
                            tempSetupUploading = true;
                            Console.WriteLine("Auto-Uploading Enabled.");
                            Thread.Sleep(1300);
                        }
                        if (inp1.Contains("no"))
                        {
                            tempSetupUploading = false;
                            Console.WriteLine("Auto-Uploading Disabled.");
                            Thread.Sleep(1300);
                        }

                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("USELESS: Streamlink HLS timeout in SECONDS. default is 1800Sec - 30 mins");
                        Console.WriteLine("Type a higher number if your internet is slow, if not type, 60");
                        Console.ResetColor();
                        hlsinterval = int.Parse(Console.ReadLine());

                        Thread.Sleep(1000);
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Do you want to Enable Video Uploaded notifications, by sending an email?");
                        Console.WriteLine("Don't fucking worry that we will steal your pass/email, you can see the source code");
                        Console.WriteLine("via GitHub or if you dont even trust that, just decompile the .EXE with a .NET decompiller");
                        Console.WriteLine("The data is stored on you'r hard drive.\n");
                        Console.ResetColor();
                        Console.WriteLine("To choose type, yes or no, in lowercase");

                        string inp2 = Console.ReadLine();
                        if (inp2.Contains("yes"))
                        {
                            tempSetupNotifications = true;
                            Console.WriteLine("Email-Notifications Enabled.\n");
                            Thread.Sleep(1300);
                            Console.WriteLine("Enter the Gmail account that the emails are sending from to your main Email. Only Gmail is supported");
                            tempEmail = Console.ReadLine();
                            Console.WriteLine("Enter the Password for Gmail you just entered.");
                            tempPass = Console.ReadLine();
                            Console.WriteLine("Enter an the email adress to send the notifications.");
                            tempSendTo = Console.ReadLine();
                        }
                        if (inp2.Contains("no"))
                        {
                            tempSetupNotifications = false;
                            Console.WriteLine("Email-Notifications Disabled.");
                            Thread.Sleep(1300);
                        }



                        using (StreamWriter sw = File.CreateText(log.MainPath))
                        {
                            if(tempSetupNotifications == false)
                            {
                                sw.Write(tempSetupUploading.ToString() + "\n" + hlsinterval + "\nDisabled\nDisabled\nDisabled");
                                sw.Close();
                            }

                            if(tempSetupNotifications == true)
                            {
                                sw.Write(tempSetupUploading.ToString() + "\n" + hlsinterval + "\n" + tempEmail + "\n" + tempPass + "\n" + tempSendTo);
                                sw.Close();
                            }

                        }


                    }
                    if (log.ConfigCheckExist(log.NotDomePath) == false)
                    {
                        using (StreamWriter sw = File.CreateText(log.NotDomePath))
                        {
                            sw.Write("");
                            sw.Close();
                        }
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
                sw.Close();
            }

            Thread.Sleep(1000);
            Console.Clear();
            Console.WriteLine("Setup is Done.\n" + string.Join("\n", ints));
        }



        int urllines;
        int intlines;
        string URLConf;
        string INTConf;
        public string EmailAddr;
        public string EmailPass;
        public string EmailSendTo;
        public void configRead()
        {
            try
            {
                string mainpathtext = File.ReadAllText(log.MainPath);
                URLConf = System.IO.File.ReadAllText(log.URLpath);
                INTConf = System.IO.File.ReadAllText(log.INTpath);
                urllines = log.CountLines(log.URLpath);
                intlines = log.CountLines(log.INTpath);  //FUCKED SHIT GOES HERE
                hlsinterval = int.Parse(GetLine(mainpathtext, 2));
                EmailAddr = GetLine(mainpathtext, 3);
                EmailPass = GetLine(mainpathtext, 4);
                EmailSendTo = GetLine(mainpathtext, 5);
                int linecount = log.CountLines(log.NotDomePath);
                string txt = File.ReadAllText(log.NotDomePath);
                for(int i = 1; i <= linecount; i++)
                {
                    log.notDoneList.Add(GetLine(txt, i));
                }

                string[] logno = log.notDoneList.ToArray();
                Console.WriteLine("\n");
                Console.WriteLine("UPLOAD QUEUE: " + log.CountLines(log.NotDomePath));
                Console.WriteLine("-----------");
                string txt1 = "";
                foreach (var item in logno)
                {
                    txt1 += item + "\n";
                }
                Console.WriteLine(txt1.Trim());
                Console.WriteLine("-----------");
                Console.WriteLine("\n");
                Console.WriteLine("=================\n\n Info from config file\n");
                Console.WriteLine("Auto Uploading: " + ch.AutoUploading());
                Console.WriteLine("HLS Timeout: " + hlsinterval);

                if (EmailAddr == "Disabled" && EmailPass == "Disabled" && EmailSendTo == "Disabled")
                {
                    Console.WriteLine("Email-Notifications: False");
                    Global_EmailNotifications = false;
                }
                else
                {
                    Global_EmailNotifications = true;
                    Console.WriteLine("Email-SendFrom: " + EmailAddr);
                    Console.WriteLine("Email-SendTo: " + EmailSendTo);
                }
                Console.WriteLine("");
                Console.WriteLine(URLConf);
                Console.WriteLine("");
                Console.WriteLine(INTConf);

                //Console.WriteLine(string.Format("URL LINES: {0}\nINTERVAL LINES: {1}", urllines, intlines));
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


        public void SendEmail(string Message)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress(EmailAddr);
            mail.To.Add(EmailSendTo);
            mail.Subject = string.Format("{0} STREAM HAS BEEN UPLOADED", Message);
            mail.Body = "";
            mail.Priority = MailPriority.High;
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(EmailAddr, EmailPass);
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
            Console.WriteLine("EMAIL SENT");
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
            new Thread(async () =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    Checker cho = new Checker();
                    await cho.check(URL, HlsTimeout);
                    Thread.Sleep(Delay);
                }

            }).Start();
        }

        public string GetLine(string text, int lineNo)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }

    }
}
