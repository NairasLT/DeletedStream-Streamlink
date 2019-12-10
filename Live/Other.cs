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
        //Upload upl = new Upload();

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