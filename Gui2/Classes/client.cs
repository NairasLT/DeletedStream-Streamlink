using Gui2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using static Gui2.Helpers.ConfigHelper;
using System.Diagnostics;
using System.Management;
using System.IO;
using YoutubeExplode.Channels;
using static Gui2.Helpers.ClientHelper;
using static Gui2.Helpers.WebSigHeaderV2;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Windows.Controls;

namespace Gui2.Classes
{
    class client
    {
        /// <summary>
        /// Listbox where to display the streamers, if you dont want this feature pass new Listbox Object.
        /// </summary>
        /// <param name="CurrentlyStreamingDisplay"></param>
        public client(ListBox CurrentlyStreamingDisplay)
        {
            streaming = new CurrentlyStreaming(CurrentlyStreamingDisplay);
        }
        CurrentlyStreaming streaming;
        public void Start()
        {
            var cfg = new ConfigSys(LocalData);

            SaveInfo save = cfg.Read();

            if (save.settings.StreamsFolder == "Local Folder") //Change to do this When Saving.
                save.settings.StreamsFolder = StreamsFolder;

            foreach (var streamer in save.Streamers)
            {
                try
                {
                    Thread thr = new Thread(() => StartStreamer(streamer, save.settings.StreamsFolder, save.settings.Clientsecrets));
                    thr.Start();
                }
                catch (Exception x) { MessageBox.Show($"Error starting Streamer {streamer.Website} Thread, Are paths correctly set?\n{x.Message}"); }
            }
        }

        private void StartStreamer(Streamer streamer, string SavePath, string SecretsName)
        {
            YoutubeClient youtubeClient = new YoutubeClient();
            Console.WriteLine($"Started {streamer.Website}");
            while (true)
            {
                int delay = (int)TimeSpan.FromMinutes(streamer.MinuteInterval).TotalMilliseconds;

                var cfg = new ConfigSys(LocalData);
                if (streamer.platform == StreamerPlatform.YouTube)
                {
                    var Result = BroadcastIdFromChannelId(streamer.Website).Result;

                    if (Result.ResultStatus == Status.Fail || Result.Result == "live_stream" || Result.Result == null)
                    {
                        Thread.Sleep(delay);
                        continue;
                    }

                    if (Result.ResultStatus == Status.Success) //Id not link
                    {
                        var metadata = youtubeClient.Videos.GetAsync(Result.Result).Result; //Video title, etc.

                        var FileNames = new FileNamePurify(ThumbnailsFolder, StreamManifestFolder, SavePath, metadata);

                        new WebClient().DownloadFile(metadata.Thumbnails.MaxResUrl, FileNames.ThumbnailPath); //Idk fucks up 
                        StreamlinkDownloadStream(FileNames.StreamPath, $"https://www.youtube.com/channel/{streamer.Website}/live").Wait();
                        SavedVideo svi = new SavedVideo(metadata, FileNames.StreamPath, FileNames.ThumbnailPath, false, StreamerPlatform.YouTube);
                        File.WriteAllText(FileNames.StreamInfoPath, JsonConvert.SerializeObject(svi));

                        if (cfg.Read().settings.Uploading)
                        {
                            YouTubeUploadStartThread(svi, SecretsName);
                        }
                    }
                }
                if (streamer.platform != StreamerPlatform.YouTube)
                {
                    string StreamPath = $"{SavePath}{DateTime.Now.Year} {DateTime.Now.Month} {DateTime.Now.Day} [{DateTime.Now.Hour} Hour {DateTime.Now.Minute} min {DateTime.Now.Second} s].mp4";
                    string StreamInfoPath = $"{StreamManifestFolder}\\{DateTime.Now.Year} {DateTime.Now.Month} {DateTime.Now.Day} [{DateTime.Now.Hour} Hour {DateTime.Now.Minute} min {DateTime.Now.Second} s].streaminfo";
                    string Title = $"{DateTime.Now.Year} {DateTime.Now.Month} {DateTime.Now.Day} [{DateTime.Now.Hour} Hour {DateTime.Now.Minute} min {DateTime.Now.Second} s]";
                    StreamlinkDownloadStream(StreamPath, streamer.Website).RunSynchronously();
                    File.WriteAllText(StreamInfoPath, JsonConvert.SerializeObject(new SavedVideo(null, StreamPath, null, false, StreamerPlatform.Other)));

                    if (cfg.Read().settings.Uploading)
                        YouTubeUploadStartThread(SecretsName, Title, string.Empty, StreamPath);
                }

                Thread.Sleep(delay);
            }
        }





        public void YouTubeUploadStartThread(string SecretsName, string Title, string Description, string Path)
        {
            new Thread(() =>
            {
                try
                {
                    var upl = new Upload(ClientSecretsFolder + SecretsName, "user");
                    upl.Init().Wait();
                    upl.Start(Title, Description, Path).Wait();
                }
                catch (Exception)
                {
                    Task.Delay(TimeSpan.FromHours(2));
                    YouTubeUploadStartThread(SecretsName, Title, Description, Path);
                }
            }).Start();

        }
        public void YouTubeUploadStartThread(SavedVideo Meta, string SecretsName)
        {
            new Thread(() =>
            {
                try
                {
                    var upl = new Upload(ClientSecretsFolder + SecretsName, "user");
                    upl.Init().Wait();
                    upl.Start(Meta).Wait();
                }
                catch (Exception)
                {
                    Thread.Sleep(TimeSpan.FromHours(2));
                    YouTubeUploadStartThread(Meta, SecretsName);
                }
            }).Start();

        }


        private async Task StreamlinkDownloadStream(string FilePath, string Url)
        {
            var streamlink = new ProcessStartInfo
            {
                FileName = StreamlinkFile,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Arguments = $" --hls-live-restart -o \"{FilePath}\" \"{Url}\" best",
                UseShellExecute = false,
                RedirectStandardOutput = false,
                CreateNoWindow = false
            };
            Process process = new Process();
            process.StartInfo = streamlink;
            process.Start();
            await Task.Run(() => process.WaitForExit());
        }
    }
}
