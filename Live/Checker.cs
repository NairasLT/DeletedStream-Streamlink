using Live;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

    class Checker
    {
        public string fromWhen = "99:00:00";
        public string filename; // DYNAMIC
        public string message; // 
        public bool AutoUploading;
    //Config conf = new Config();
    Upload upl = new Upload();



    public async Task check(string ChannelURL, int hlstimeout)
        {

        Console.WriteLine(DateTime.Now + string.Format(" Started Thread For Channel URL: {0}", ChannelURL));
        // EXECUTINA COMMAND LINE, VISADA KEICIASIA FILENAME VALUE I DABARTINE DATA

        filename = DateTime.Now.ToString();
        filename = filename.Replace(':', '-').Replace('/', '-').Replace(' ', '_'); // REGEX
        filename += ".mp4"; // naujas
        string final = string.Format("streamlink --hls-live-restart --hls-timeout {0} --hls-segment-threads 7 -o " + filename + " " + ChannelURL + " best", hlstimeout); // COMMAND - URL
        Process cmd = new Process();
        cmd.StartInfo.FileName = "cmd.exe";
        cmd.StartInfo.RedirectStandardInput = true;
        cmd.StartInfo.RedirectStandardOutput = true;
        cmd.StartInfo.CreateNoWindow = true; // BULLSHIT
        cmd.StartInfo.UseShellExecute = false;
        cmd.Start();
        cmd.StandardInput.WriteLine(final);
        cmd.StandardInput.Flush();
        cmd.StandardInput.Close();
        message = cmd.StandardOutput.ReadToEnd();
        Console.WriteLine(string.Format("\n\n{0}\n\n", message));
        if (message.Contains("error"))
            {
            Console.WriteLine(DateTime.Now +" STREAM OFFLINE FOR URL: " + ChannelURL); // NERANDA RETURININA
            }
        else
        {
            if (message.Contains("Stream ended") && message.Contains(ChannelURL)) // PASIBAIGE VISI STREAMAI SU /LIVE TAGU BAIGIASI PO 5 MIN!
            {


                if (AutoUploading == true)
                {
                    Console.WriteLine("Stream Ended Now Will Be Uploading");
                    Thread t = new Thread(() => Uploade(filename));
                    t.Start();
                }
                Console.WriteLine("CREATED THREAD FOR UPLOADING NOW WILL BE CHECKING AGAIN FOR URL: " + ChannelURL);
            }
            else
            {
                Console.WriteLine("PROCCESS KILLED YOU WHORE!!!! / Or Rate Limited even better!!!");
                Thread t1 = new Thread(() => Uploade(filename));
                t1.Start();
            }
        }





        }


    public void Uploade(string ChannelURL)
    {
        Console.WriteLine(ChannelURL); // WENT OFFLINE
        Console.WriteLine("---> TRYING TO UPLOAD!");
        upl.Run(filename); // REIKIA SUTAISYKTI GET TIKRAI VEIKTU REPEAT UPLOAD ON THREAD!!!
    }



}

 

