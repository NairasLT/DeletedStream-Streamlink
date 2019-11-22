using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Live
{
    class Other
    {
        Checker ch = new Checker();
        Upload upl = new Upload();

        public string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\LiveConfig";
        public string URLpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\LiveConfig\\configURL.txt";
        public string INTpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\LiveConfig\\configINTERVAL.txt";
        public string MainPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\LiveConfig\\Main.txt";
        public string NotDomePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\LiveConfig\\NotUploadedList.txt";
        public List<string> notDoneList = new List<string>();


        public bool ConfigCheckExist(string path)
        {
            if (!File.Exists(path))  // IF DOSENT EXIST
            {
                return false;
            }

            else // IF EXISTS
            {
                return true;
            }
        }


        public async void commands()
        {
            string inp = Console.ReadLine();
            if (inp.Contains("uploadvideo"))
            {
                Console.Clear();
                Thread.Sleep(1000);
                Console.WriteLine("Enter a Path, or the filename if the video is in the same folder as the live.exe\n   example: myvideo.mp4");
                string inp2 = Console.ReadLine();
                await upl.Run(inp2);
            }
            if (inp.Contains("checkonce"))
            {
                Console.Clear();
                Thread.Sleep(1000);
                Console.WriteLine("Enter the URL, you can enter here /watch urls not channel urls with /live tag\n   Cause they are checked one time.");
                string urler = Console.ReadLine();
                Console.WriteLine("Write HLS timeout, enter 60 if you dont know what this means.");
                int customhls = int.Parse(Console.ReadLine());
                await ch.check(urler, customhls);
            }
        }

        public int CountLines(string Path)
        {
            int lineCount = File.ReadLines(Path).Count();
            return lineCount;
        }



        /*            foreach (var item in ReadedLines)
            {
                Console.WriteLine(item.ToString());
            }*/

    }
}