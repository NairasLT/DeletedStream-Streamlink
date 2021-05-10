using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using RestSharp;
class TrovoPlugin : IPlugin
{
    public TrovoPlugin(string name, TimeSpan delay) // DEPRACTED
    {
        Name = name;
        Timeout = delay;
    }

    public string Name { get; set; }
    public TimeSpan Timeout { get; set; }

    internal Scrape ISLIVE = new Scrape("\"isLive\":", ",");
    internal Scrape TITLE = new Scrape("property=\"og:description\" content=\"", "\"");
    internal Scrape DESCRIPTION = new Scrape("info:\"", "\"");


    public async Task<string> RequestCurrentStatus()
    {
        try
        {
            var client = new RestClient($"https://trovo.live/{Name}");
            client.Timeout = -1;
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";
            var request = new RestRequest(Method.GET);
            IRestResponse PageResponse = await client.ExecuteAsync(request);

            if (PageResponse.Content == null)
                return null;

            string BoolText = ScrapeBit.FirstFrom(PageResponse.Content, ISLIVE);
            if (BoolText == null)
                return null;

            bool status = bool.Parse(BoolText.ToLower());

            if (status) return PageResponse.Content;
            else return null;
        }
        catch (Exception) { return null; }

    }
    public async Task RunInfinite()
    {
        while (true)
        {
            await Run();
            await Task.Delay(Timeout);
        }
    }

    public async Task Run()
    {
        try
        {
            string PageResponse = await RequestCurrentStatus();
            if (PageResponse != null)
            {
                Player DownloadInfo = Player.RetrievePlayer(PageResponse);
                if (DownloadInfo.Quality == Quality.NotFound)
                    return;

                string Title = ScrapeBit.FirstFrom(PageResponse, TITLE);
                string Path = FilePaths.GetLivestreamsPath(FileName.Purify($"{Title} [{DateTime.Now.Ticks.GetHashCode()}].mp4"));
                string Description = ScrapeBit.FirstFrom(PageResponse, DESCRIPTION);


                Console.WriteLine($"Found Livestream with Title: {Title} and Quality: {DownloadInfo.Quality}");
                await Download(DownloadInfo.Url, Path);

                var Upload = new Youtube(FilePaths.SecretsFile);
                await Upload.Init();
                _ = Upload.UploadWithRetry(new YoutubeUpload() { LivestreamPath = Path, Title = Title, Description = Description, ThumbnailPath = null }, TimeSpan.FromHours(3));

            }
        }

        catch (Exception x) { Console.WriteLine($"Error in Check loop, Exception occurred: {x.Message}, please restart"); }
    }
    internal async Task Download(string Url, string Path)
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