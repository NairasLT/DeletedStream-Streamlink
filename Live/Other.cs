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
        Upload upl = new Upload();
        Checker ch = new Checker();
        public string URLpath;
        public string FolderPath;
        public string INTpath;
        public string MainPath;
        public void Uploader(string filename)
        {
            var uple = new Upload();
            Thread thr = new Thread(upload);
            thr.Start();

            void upload()
            {
                uple.Run(filename);
                Console.WriteLine(filename);
            }

        }

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

        public void commands()
        {
            string inp = Console.ReadLine();
            if (inp.Contains("uploadvideo"))
            {
                Console.Clear();
                Thread.Sleep(1000);
                Console.WriteLine("Enter a Path, or the filename if the video is in the same folder as the live.exe\n   example: myvideo.mp4");
                string inp2 = Console.ReadLine();
                upl.Run(inp2);
            }
            if (inp.Contains("checkonce"))
            {
                Console.Clear();
                Thread.Sleep(1000);
                Console.WriteLine("Enter the URL, you can enter here /watch urls not channel urls with /live tag\n   Cause they are checked one time.");
                string urler = Console.ReadLine();
                Console.WriteLine("Write HLS timeout, enter 60 if you dont know what this means.");
                int customhls = int.Parse(Console.ReadLine());
                ch.check(urler, customhls);
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