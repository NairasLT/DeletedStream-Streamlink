using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Live
{
    class Program
    {
        static void Main(string[] args)
        {
            var frm = new Program();
            frm.starter();
        }

        public string customurl = "";
        public bool Status = true;
        public bool customEnabled = false;
        Config conf = new Config();
        Upload upl = new Upload();
        Checker ch = new Checker();
        Other oth = new Other();
        public void starter() // REIKIA SUTAISYTI TIMINGS
        {
            conf.configURL();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("<-------------->\n");
            Console.WriteLine("1. Streamlink Timeout 30 minutes to prevent uploading in parts if stream went offline for a minute");
            Console.WriteLine("2. If You want to check a youtube channel you MUST follow this format \n   https://www.youtube.com/channel/thechannelnamehere/live <-- live tag is very important!\n");
            Console.WriteLine("3. Auto uploading is Enabled the Google Api files should be in the same folder as the live.exe\n");
            Console.WriteLine("4. To auto upload to Youtube get client_secrets.json and move it to 'c:\\client_secrets.json'\n   the first upload will ask to\n   choose the channel to upload to, make sure Chrome isn't open when the channel to upload is not chosen\n");
            Console.WriteLine("5. To get client_secrets.json go to http://console.developers.google.com/ \n   create a project -> create crendentials -> Oauth2 -> download json rename it to 'client_secrets.json'\n   or just google it how to.\n");
            Console.WriteLine("6. If you have any problems, delete the LiveConfig folder on the desktop\n   if not fixed create an issue on github.\n");
            Console.WriteLine("7. NOTE: if you are auto-uploading to youtube there is a QUOTA limit 2-5 videos every day.\n   NOTE: Make sure to update from GitHub:\n   https://github.com/NairasLT/Checking-Downloading-Uploading-Livestream_using_streamlink");
            Console.WriteLine("                                  Version: 1.1 cmds: uploadvideo, checkonce");
            Console.WriteLine("<-------------->\n");
            Console.ResetColor();
            Console.WriteLine("Press Enter to Start Checking.");

            oth.commands();
            conf.configRead();



            while (true)
            {
                oth.commands();
            }


        }






    }
}
