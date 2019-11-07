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
        public string message; // DYNAMIC
        public int tries;
          Upload upl = new Upload();
          Other log = new Other();


        public async Task check(string ChannelURL)
        {

        new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;

        Console.WriteLine(string.Format("Started Thread For Channel URL: {0}", ChannelURL));
        // EXECUTINA COMMAND LINE, VISADA KEICIASIA FILENAME VALUE I DABARTINE DATA

        filename = DateTime.Now.ToString() + "";
        filename = filename.Replace(':', '-').Replace('/', '-').Replace(' ', '_'); // REGEX
        filename += ".mp4"; // naujas
        string final = "streamlink --hls-live-restart -o " + filename + " " + ChannelURL + " best"; // COMMAND - URL
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

            if (message.Contains("error"))
            {
                Console.WriteLine("STREAM OFFLINE FOR URL: " + ChannelURL);
            }

            else
            {

                Console.WriteLine("Ended recording stream went offline\nretrying for Channel URL: " + ChannelURL); // WENT OFFLINE
                Console.WriteLine("UPLOADING TO YOUTUBE, CREATING THREAD FOR JOB");

                Thread thr = new Thread(UploadVideo);
                thr.Start();
                void UploadVideo()
                {
                    void container()
                    {
                        Console.WriteLine("TRYING TO UPLOAD");
                        upl.Run(filename); // REIKIA SUTAISYKTI GET TIKRAI VEIKTU REPEAT UPLOAD ON THREAD!!!
                    }
                    container();
                }




                Console.WriteLine("CREATED THREAD, RETURNING TO CHECKING");
            }


        }).Start();

      }

 }

