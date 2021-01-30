using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;
using YoutubeExplode;

public class ActiveChannel
{
    public ActiveChannel(string channelId, TimeSpan timeOut)
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
            var Status = await GetLivestreamStatusFromChannelId(ChannelId);

            if (!Status.IsLivestreaming)
            {
                await Sleep();
                continue;
            }

            var ytExplode = new YoutubeClient();
            var metadata = await ytExplode.Videos.GetAsync(Status.videoId);
            var StreamObject = new LivestreamObject(metadata, FilePaths.GetThumbnailPath(FileName.Purify($"{metadata.Title} [{DateTime.Now.Ticks.GetHashCode()}].jpeg")), FilePaths.GetLivestreamsPath(FileName.Purify($"{metadata.Title} [{DateTime.Now.Ticks.GetHashCode()}].mp4")));
            try
            {
                new WebClient().DownloadFile(metadata.Thumbnails.MaxResUrl, StreamObject.ThumbnailPath);
            }
            catch (Exception) { }
            await Streamlink.Download(StreamObject.LivestreamPath, metadata.Url);

            var Upload = new Upload(FilePaths.SecretsFile);
            await Upload.Init();
            _ = Upload.CreateWithRetry(StreamObject, TimeSpan.FromHours(3));

            await Sleep();
        }
    }


    public async Task RunOnce()
    {
        var Status = await GetLivestreamStatusFromChannelId(ChannelId);

        if (!Status.IsLivestreaming)
            return;

        var ytExplode = new YoutubeClient();
        var metadata = await ytExplode.Videos.GetAsync(Status.videoId);
        var StreamObject = new LivestreamObject(metadata, FilePaths.GetThumbnailPath(FileName.Purify($"{metadata.Title} [{DateTime.Now.Ticks.GetHashCode()}].jpeg")), FilePaths.GetLivestreamsPath(FileName.Purify($"{metadata.Title} [{DateTime.Now.Ticks.GetHashCode()}].mp4")));

        try
        { new WebClient().DownloadFile(metadata.Thumbnails.MaxResUrl, StreamObject.ThumbnailPath); }
        catch (Exception) { }

        await Streamlink.Download(StreamObject.LivestreamPath, metadata.Url);

        var Upload = new Upload(FilePaths.SecretsFile);
        await Upload.Init();
        _ = Upload.CreateWithRetry(StreamObject, TimeSpan.FromHours(3));

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
