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