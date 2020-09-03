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

namespace Gui2.Classes
{
    class client
    {
        public async Task Client()
        {

            var cfg = new ConfigSys(LocalData);

            SaveInfo save = cfg.ReadInfo();

            if (save == null)
                return;

            if (save.settings.Streams_Save_Folder == string.Empty)
                save.settings.Streams_Save_Folder = StreamsFolder;

            foreach (var streamer in save.Streamers)
            {
                Thread strmthr = new Thread(() => StartStreamer(streamer, save.settings.Streams_Save_Folder)); //Nezinau kaip cia geriausiai butu nes yra async methods
                strmthr.Start();
            }
        }

        private async Task StartStreamer(Streamer streamer, string SavePath)
        {
            YoutubeClient youtubeClient = new YoutubeClient();

            while (true)
            {
                string StreamPath = $"{SavePath}\\{DateTime.Now.Year} {DateTime.Now.Month} {DateTime.Now.Day} [{DateTime.Now.Hour} Hour {DateTime.Now.Minute} min {DateTime.Now.Second} s].mp4";
                string ThumbnailPath = $"{ThumbnailsFolder}\\{DateTime.Now.Year} {DateTime.Now.Month} {DateTime.Now.Day} [{DateTime.Now.Hour} Hour {DateTime.Now.Minute} min {DateTime.Now.Second} s].jpeg";
                string StreamInfoPath = $"{StreamManifestFolder}\\{DateTime.Now.Year} {DateTime.Now.Month} {DateTime.Now.Day} [{DateTime.Now.Hour} Hour {DateTime.Now.Minute} min {DateTime.Now.Second} s].streaminfo";
                int delay = (int)TimeSpan.FromMinutes(streamer.MinuteInterval).TotalMilliseconds;


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
                        await new WebClient().DownloadFileTaskAsync(metadata.Thumbnails.MaxResUrl, ThumbnailPath);
                        await StreamlinkDownloadStream(SavePath, $"https://www.youtube.com/channel/{streamer.Website}/live");
                        SavedVideo svi = new SavedVideo(metadata, StreamPath, ThumbnailPath, false, StreamerPlatform.YouTube);
                        File.WriteAllText(StreamInfoPath, JsonConvert.SerializeObject(svi));
/*                        var upl = new Upload(ClientSecretsFolder + "client_secrets.json", "user");
                        await upl.Init();
                        await upl.Start(svi);*/
                    }
                }


                if (streamer.platform != StreamerPlatform.YouTube)
                {
                    await StreamlinkDownloadStream(StreamPath, streamer.Website);
                    File.WriteAllText(StreamInfoPath, JsonConvert.SerializeObject(new SavedVideo(null, StreamPath, null, false, StreamerPlatform.Other)));
                }
                 


                await Task.Delay(delay);
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
