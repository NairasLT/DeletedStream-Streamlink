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
        public async Task Start()
        {
            var cfg = new ConfigSys(LocalData);

            SaveInfo save = cfg.Read();

            if (save.settings.StreamsFolder == "Local Folder") //Change to do this When Saving.
                save.settings.StreamsFolder = StreamsFolder;

            foreach (var streamer in save.Streamers)
            {
                try
                {
/*                    Thread strmthr = new Thread(() => StartStreamer(streamer, save.settings.StreamsFolder, save.settings.Clientsecrets));
                    strmthr.Start();*/
                    StartStreamer(streamer, save.settings.StreamsFolder, save.settings.Clientsecrets);
                }
                catch(Exception x) { MessageBox.Show($"Error starting Streamer {streamer.Website} Thread, Are paths correctly set?\n{x.Message}"); }
            }
        }

        private async Task StartStreamer(Streamer streamer, string SavePath, string SecretsName)
        {
            YoutubeClient youtubeClient = new YoutubeClient();
            Console.WriteLine($"Started {streamer.Website}");
            while (true)
            {
                int delay = (int)TimeSpan.FromMinutes(streamer.MinuteInterval).TotalMilliseconds;

                var cfg = new ConfigSys(LocalData);
                if(streamer.platform == StreamerPlatform.YouTube)
                {
                    var Result = await BroadcastIdFromChannelId(streamer.Website);

                    if (Result.ResultStatus == Status.Fail || Result.Result == "live_stream" || Result.Result == null)
                    {
                        await Task.Delay(delay);
                        continue;
                    }

                    if (Result.ResultStatus == Status.Success) //Id not link
                    {
                        var metadata = await youtubeClient.Videos.GetAsync(Result.Result); //Video title, etc.

                        var FileNames = new FileNamePurify(ThumbnailsFolder, StreamManifestFolder, SavePath, metadata);

                        streaming.AddStreamer(streamer);
                        await new WebClient().DownloadFileTaskAsync(metadata.Thumbnails.MaxResUrl, FileNames.ThumbnailPath); //Idk fucks up 
                        await StreamlinkDownloadStream(FileNames.StreamPath, $"https://www.youtube.com/channel/{streamer.Website}/live");
                        SavedVideo svi = new SavedVideo(metadata, FileNames.StreamPath, FileNames.ThumbnailPath, false, StreamerPlatform.YouTube);
                        File.WriteAllText(FileNames.StreamInfoPath, JsonConvert.SerializeObject(svi));
                        streaming.RemoveStreamer(streamer);

                        if (cfg.Read().settings.Uploading)
                            YouTubeUpload(svi, SecretsName);
                    }
                }
                if (streamer.platform != StreamerPlatform.YouTube)
                {
                    string StreamPath = $"{SavePath}{DateTime.Now.Year} {DateTime.Now.Month} {DateTime.Now.Day} [{DateTime.Now.Hour} Hour {DateTime.Now.Minute} min {DateTime.Now.Second} s].mp4";
                    string StreamInfoPath = $"{StreamManifestFolder}\\{DateTime.Now.Year} {DateTime.Now.Month} {DateTime.Now.Day} [{DateTime.Now.Hour} Hour {DateTime.Now.Minute} min {DateTime.Now.Second} s].streaminfo";
                    string Title = $"{DateTime.Now.Year} {DateTime.Now.Month} {DateTime.Now.Day} [{DateTime.Now.Hour} Hour {DateTime.Now.Minute} min {DateTime.Now.Second} s]";
                    streaming.AddStreamer(streamer);
                    await StreamlinkDownloadStream(StreamPath, streamer.Website);
                    File.WriteAllText(StreamInfoPath, JsonConvert.SerializeObject(new SavedVideo(null, StreamPath, null, false, StreamerPlatform.Other)));
                    streaming.RemoveStreamer(streamer);

                    if (cfg.Read().settings.Uploading)
                        YouTubeUpload(SecretsName, Title, string.Empty, StreamPath);
                }

                await Task.Delay(delay);
            }
        }





        public async Task YouTubeUpload(string SecretsName, string Title, string Description, string Path)
        {

            try
            {
                var upl = new Upload(ClientSecretsFolder + SecretsName, "user");
                await upl.Init();
                await upl.Start(Title, Description, Path);
            }
            catch (Exception)
            {
                await Task.Delay(TimeSpan.FromHours(1));
                YouTubeUpload(SecretsName, Title, Description, Path);
            }
        }
        public async Task YouTubeUpload(SavedVideo Meta, string SecretsName)
        {
            try
            {
                var upl = new Upload(ClientSecretsFolder + SecretsName, "user");
                await upl.Init();
                await upl.Start(Meta);
            }
            catch (Exception)
            {
                await Task.Delay(TimeSpan.FromHours(2));
                YouTubeUpload(Meta, SecretsName);
            }
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
