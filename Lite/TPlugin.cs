using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using RestSharp;
class TrovoStreamer
{
    public TrovoStreamer(string name, TimeSpan delay)
    {
        Name = name;
        Delay = delay;
    }
    public string Name { get; set; }
    public TimeSpan Delay { get; set; }

    private async Task Sleep()
    {
        await Task.Delay(Delay);
    }

    void AddRestHeaders(ref RestRequest request)
    {
        request.AddHeader("authority", "trovo.live");
        request.AddHeader("cache-control", "max-age=0");
        request.AddHeader("upgrade-insecure-requests", "1");
        request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
        request.AddHeader("service-worker-navigation-preload", "true");
        request.AddHeader("sec-fetch-site", "same-origin");
        request.AddHeader("sec-fetch-mode", "navigate");
        request.AddHeader("sec-fetch-user", "?1");
        request.AddHeader("sec-fetch-dest", "document");
        request.AddHeader("accept-language", "en-US,en;q=0.9");
        request.AddHeader("if-modified-since", "Sun, 06 Dec 2020 19:03:54 GMT");
    }


    /// <summary>
    /// If the streamer is live returns the page content.
    /// </summary>
    /// <returns></returns>
    public async Task<string> RequestCurrentStatus()
    {
        var client = new RestClient($"https://trovo.live/{Name}");
        client.Timeout = -1;
        client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";
        var request = new RestRequest(Method.GET);
        AddRestHeaders(ref request);
        IRestResponse PageResponse = await client.ExecuteAsync(request);

        if (PageResponse.Content == null)
            return null;

        string BoolText = ScrapeBit.FirstString(PageResponse.Content, "\"isLive\":", ",\"");
        if (BoolText == null)
            return null;

        bool status = bool.Parse(BoolText.ToLower());

        if (status) return PageResponse.Content;
        else return null;
    }

    public async Task RunInfinite()
    {
        try
        {
            while (true)
            {
                var client = new RestClient($"https://trovo.live/{Name}");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                AddRestHeaders(ref request);
                client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";

                IRestResponse PageResponse = await client.ExecuteAsync(request);

                if (PageResponse.Content == null)
                {
                    await Sleep();
                    continue;
                }

                string BoolText = ScrapeBit.FirstString(PageResponse.Content, "\"isLive\":", ",\"");
                if (BoolText == null)
                {
                    await Sleep();
                    continue;
                }

                bool IsLive = bool.Parse(BoolText.ToLower());

                if (IsLive)
                {
                    Player DownloadInfo = Player.RetrievePlayer(PageResponse.Content);
                    if (DownloadInfo.Quality == Quality.NotFound)
                        continue;

                    string Title = ScrapeBit.FirstString(PageResponse.Content, "\"StreamerPrivilegeInfo\",\"", "\",");
                    string Path = FilePaths.GetLivestreamsPath(FileName.Purify($"{Title} [{DateTime.Now.Ticks.GetHashCode()}].mp4"));

                    Console.WriteLine($"Found Livestream with Title: {Title} and Quality: {DownloadInfo.Quality}");
                    await Download(DownloadInfo.Url, Path);

                    var Upload = new Upload(FilePaths.SecretsFile);
                    await Upload.Init();
                    _ = Upload.CreateWithRetry(Title, $"Trovo Livestream\nUpload Time:\n{DateTime.UtcNow}", Path, TimeSpan.FromHours(3));

                }
                await Sleep();
            }
        }
        catch (Exception x) { Console.WriteLine($"Error in Check loop, Exception occurred: {x.Message}, please restart"); }

    }


    public async Task RunOnce()
    {
        try
        {
            string PageResponse = await RequestCurrentStatus();
            if (PageResponse != null)
            {
                Player DownloadInfo = Player.RetrievePlayer(PageResponse);
                if (DownloadInfo.Quality == Quality.NotFound)
                    return;

                string Title = ScrapeBit.FirstString(PageResponse, "\"StreamerPrivilegeInfo\",\"", "\",");
                string Path = FilePaths.GetLivestreamsPath(FileName.Purify($"{Title} [{DateTime.Now.Ticks.GetHashCode()}].mp4"));

                Console.WriteLine($"Found Livestream with Title: {Title} and Quality: {DownloadInfo.Quality}");
                await Download(DownloadInfo.Url, Path);

                var Upload = new Upload(FilePaths.SecretsFile);
                await Upload.Init();
                _ = Upload.CreateWithRetry(Title, $"Trovo Livestream\nUpload Time:\n{DateTime.UtcNow}", Path, TimeSpan.FromHours(3));

            }
        }
        catch (Exception x) { Console.WriteLine($"Error in Check loop, Exception occurred: {x.Message}, please restart"); }

    }



    private async Task Download(string Url, string Path)
    {
        var Web = new WebClient();
        try { await Web.DownloadFileTaskAsync(new Uri(Url), Path); } catch (Exception x) { Console.WriteLine($"Exception Occured while Downloading Livestream: {x.Message}"); return; }
    }
}
public class Player
{
    public Player(string url, Quality quality)
    {
        Url = url;
        Quality = quality;
    }
    public static Player RetrievePlayer(string PageContent)
    {
        IList<Player> PlayerQualityTypes = new List<Player>();
        Bitrate[] Bitrates = { Bitrate.Create(5000, Quality.High), Bitrate.Create(0, Quality.MediumHighest), Bitrate.Create(2500, Quality.Medium), Bitrate.Create(1500, Quality.Low), Bitrate.Create(600, Quality.VeryLow) };


        foreach (var Bitrate in Bitrates)
        {
            string Url = ScrapeBit.FirstString(PageContent, "{\"bitrate\":" + Bitrate.amount + ",\"playUrl\":\"", "\","); // FHD High Bitrate

            if (Url == null)
                continue;
            else return new Player(Url, Bitrate.quality);
        }
        return new Player(null, Quality.NotFound);
    }
    public string Url { get; set; }
    public Quality Quality { get; set; }
}

public struct Bitrate
{
    public Bitrate(int amount, Quality quality)
    {
        this.amount = amount;
        this.quality = quality;
    }
    public int amount { get; set; }
    public Quality quality { get; set; }
    public static Bitrate Create(int bitrate, Quality quality)
    {
        return new Bitrate(bitrate, quality);
    }

}
public enum Quality
{
    High = 5000,
    MediumHighest = 0,
    Medium = 2500,
    Low = 1500,
    VeryLow = 600,
    NotFound = -1,
}