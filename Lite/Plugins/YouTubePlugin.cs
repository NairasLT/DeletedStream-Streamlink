using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;
using YoutubeExplode;

public class YouTubePlugin : IPlugin
{
    public YouTubePlugin(string channelId, TimeSpan timeOut)
    {
        ChannelId = channelId;
        TimeOut = timeOut;
    }

    public string ChannelId { get; set; }
    public TimeSpan TimeOut { get; set; }

    public async Task<bool> RequestCurrentStatus()
    {
        var Status = await GetLivestreamStatusFromChannelId(ChannelId);
        return Status.IsLivestreaming;
    }

    internal async Task Sleep()
    {
        await Task.Delay(TimeOut);
    }

    public async Task RunInfinite()
    {
        while (true)
        {
            await Run();
            await Sleep();
        }
        
    }

    public async Task Run()
    {
        var Status = await GetLivestreamStatusFromChannelId(ChannelId);

        if (!Status.IsLivestreaming)
            return;
        

        var ytExplode = new YoutubeClient();
        var metadata = await ytExplode.Videos.GetAsync(Status.videoId);

        var UploadInformation = new YoutubeUpload()
        {
            Title = metadata.Title,
            Description = metadata.Description,
            ThumbnailPath = FilePaths.GetThumbnailPath(FileName.Purify($"{metadata.Title} [{DateTime.Now.Ticks.GetHashCode()}].jpeg")),
            LivestreamPath = FilePaths.GetLivestreamsPath(FileName.Purify($"{metadata.Title} [{DateTime.Now.Ticks.GetHashCode()}].mp4"))
         };


        try
        { new WebClient().DownloadFile(metadata.Thumbnails.MaxResUrl, UploadInformation.ThumbnailPath); }
        catch (Exception) { }

        await Streamlink.Download(UploadInformation.LivestreamPath, metadata.Url);

        var Upload = new Youtube(FilePaths.SecretsFile);
        await Upload.Init();
        _ = Upload.UploadWithRetry(UploadInformation, TimeSpan.FromHours(3));

    }

    public static async Task<LivestreamStatus> GetLivestreamStatusFromChannelId(string ChannelId)
    {
        var client = new RestClient($"https://www.youtube.com/embed/live_stream?channel={ChannelId}");
        client.Timeout = 8000;
        var request = new RestRequest(Method.GET);
        request.AddHeader("Cookie", "PREF=cvdm=grid; VISITOR_INFO1_LIVE=a4lxAJu-9BU");
        IRestResponse response = await client.ExecuteAsync(request);

        string videoId = ScrapeBit.FirstString(response.Content, "\\\"videoId\\\":\\\"", "\\\"");
        if (videoId == null) return new LivestreamStatus(false, null);
        else return new LivestreamStatus(true, videoId);
    }
}

public class LivestreamStatus // Change to universal plugin format like trovo
{
    public LivestreamStatus(bool IsLive, string VideoId)
    {
        IsLivestreaming = IsLive;
        videoId = VideoId;
    }

    public bool IsLivestreaming { get; set; }
    public string videoId { get; set; }

}
