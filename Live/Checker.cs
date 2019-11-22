using Live;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

    class Checker
    {
        public string filename = DateTime.Now.ToString().Replace(':', '-').Replace('/', '-').Replace(' ', '_') + ".mp4"; // DYNAMIC
    public async Task check(string ChannelURL, int hlstimeout){
        try
        {
                string message;
                Thread.CurrentThread.IsBackground = true;
                Console.WriteLine(DateTime.Now + string.Format(" Started Thread For Channel URL: {0}", ChannelURL));
                string final = string.Format("streamlink --hls-live-restart --hls-timeout {0} --hls-segment-threads 7 -o " + filename + " " + ChannelURL + " best", hlstimeout); // COMMAND - URL
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true; // BULLSHIT
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.StandardInput.WriteLine(final);
                cmd.StandardInput.Close();
                message = cmd.StandardOutput.ReadToEnd();


            if (message.Contains("error: No playable streams found on this URL:") || message.Contains("error: Unable to open URL:"))
            {
                Console.WriteLine(DateTime.Now + " STREAM OFFLINE FOR URL: " + ChannelURL);
            }
            else
            {
                if (message.Contains("Stream ended"))
                {
                    Console.WriteLine(DateTime.Now + " STREAM ENDED.");
                    VarsAndFunctions afera = new VarsAndFunctions();
                    if (AutoUploading() == true)
                    {
                        Upload upl = new Upload();
                        upl.writenotDone(filename);
                    }
                  else
                    {
                        Console.WriteLine("Auto Uploading Disabled, Not Uploading. " + filename + "\nMESSAGE\n" + message + "\nMESSAGE\n");
                    }
                }
                else
                {
                    if (message.Contains("Error:\"unauthorized_client\""))
                    {
                        Console.Clear();
                        Console.WriteLine("NOT ATHUORIZED WITH GOOGLE, CHROME WINDOW SHOULD POP UP, TO SELECT TO WHICH CHANNEL TO UPLOAD!");
                    }


                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }





    }

    public bool AutoUploading()
    {

                string temp = File.ReadAllText( Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\LiveConfig\\Main.txt");
                return bool.Parse(GetLine(temp,1));
    }

    string GetLine(string text, int lineNo)
    {
        string[] lines = text.Replace("\r", "").Split('\n');
        return lines.Length >= lineNo ? lines[lineNo - 1] : null;
    }



}



