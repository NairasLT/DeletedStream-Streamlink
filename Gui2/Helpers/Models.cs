using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Videos;

namespace Gui2.Helpers
{
    class SavedVideo // Store all Structures in Models Class! todo
    {
        public SavedVideo(Video youtubemanifest, string videopath, string thumnailpath, bool uploaded, StreamerPlatform streamerPlatform)
        {
            YouTubeManifest = youtubemanifest;
            Files.VideoPath = videopath;
            Files.ThumbnailPath = thumnailpath;
            // Uploaded = uploaded;
            Platform = streamerPlatform;
        }
        public FSVideo Files;
        public Video YouTubeManifest;
        public StreamerPlatform Platform;
        //   public bool Uploaded;
    }

    struct FSVideo
    {
        public string VideoPath;
        public string ThumbnailPath;
    }

    class SaveInfo
    {
        public List<Streamer> Streamers = new List<Streamer>();
        public Settings settings = new Settings();
    }

    public class Streamer
    {

        public Streamer(string website, int IntervalInMinutes, StreamerPlatform streamerPlatform)
        {
            Website = website;
            MinuteInterval = IntervalInMinutes;
            platform = streamerPlatform;
        }

        public string Website;
        public int MinuteInterval;
        public StreamerPlatform platform;
    }

    class Settings
    {
        public string Client_Secrets = "client_secrets.json";
        public string Streams_Save_Folder = "Local Folder";
    }

    public enum StreamerPlatform
    {
        YouTube,
        DLive,
        Twitch,
        Other
    }
    public class JsonDecodeResult
    {
        public JsonDecodeResult(string result, Status status)
        {
            Result = result;
            ResultStatus = status;
        }
        public string Result;
        public Status ResultStatus;
    }
    public enum Status
    {
        Success,
        Fail
    }
}
