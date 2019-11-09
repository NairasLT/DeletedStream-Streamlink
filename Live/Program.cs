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
        Checker ch = new Checker();
        Other log = new Other();
        Upload upl = new Upload();
        Config conf = new Config();


        public void starter() // REIKIA SUTAISYTI TIMINGS
        {
            conf.configURL();
            Console.Clear();
            Console.WriteLine("streamlink must be installed, upload to youtube if failled dosen't upload, the live.exe\nmust be in the same folder with the GoogleApi Dlls\nNeed to manually then, commands: none\nTo upload video to youtube create oAuth2 credentials download them .json i c: \n pvz C:\\client_secrets.json"); // REIK PAKEIST TRY UPLOAD EVERY !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            conf.configRead();



            while (true)
            {
                Console.ReadLine();
            }


        }






    }
}
