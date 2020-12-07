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
    public async Task BeginLoop()
    {
        try
        {
            while (true)
            {
                var client = new RestClient($"https://trovo.live/{Name}");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("authority", "trovo.live");
                request.AddHeader("cache-control", "max-age=0");
                request.AddHeader("upgrade-insecure-requests", "1");
                client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";
                request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                request.AddHeader("service-worker-navigation-preload", "true");
                request.AddHeader("sec-fetch-site", "same-origin");
                request.AddHeader("sec-fetch-mode", "navigate");
                request.AddHeader("sec-fetch-user", "?1");
                request.AddHeader("sec-fetch-dest", "document");
                request.AddHeader("accept-language", "en-US,en;q=0.9");
                request.AddHeader("cookie", "_ga=GA1.1.620597506.1607281434; pgg_pvid=277130035220120621; afUserId=87b19c83-83b9-4c4d-bd18-64484bfcc93d-p; _ga_PTRRXCPFRR=GS1.1.1607281434.1.1.1607281434.0; _ga_18B1XRM1NB=GS1.1.1607281434.1.0.1607281435.0; pgg_pvid=299425075220120421");
                request.AddHeader("if-none-match", "\"10b54-NAlZVFadyBGWcpm531Uakzt50s0\"");
                request.AddHeader("if-modified-since", "Sun, 06 Dec 2020 19:03:54 GMT");
                IRestResponse PageResponse = await client.ExecuteAsync(request);

                if (PageResponse.Content == null)
                {
                    await Sleep();
                    continue;
                }

                string BoolText = ScrapeBit.String(PageResponse.Content, "\"isLive\":", ",\"");
                if (BoolText == null)
                {
                    await Sleep();
                    continue;
                }

                bool IsLive = bool.Parse(BoolText.ToLower());

                if (IsLive)
                {
                    Player DownloadInfo = Helpers.RetrievePlayer(PageResponse.Content);
                    if (DownloadInfo.Quality == Quality.NotFound)
                        continue;

                    string Title = ScrapeBit.String(PageResponse.Content, "\"StreamerPrivilegeInfo\",\"", "\",");
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
    private async Task Download(string Url, string Path)
    {
        var Web = new WebClient();
        try { await Web.DownloadFileTaskAsync(new Uri(Url), Path); } catch (Exception x) { Console.WriteLine($"Exception Occured while Downloading Livestream: {x.Message}"); return; }
    }
}
public static class Helpers
{
    public static Player RetrievePlayer(string PageContent)
    {
        IList<Player> PlayerQualityTypes = new List<Player>();
        Bitrate[] Bitrates = { Bitrate.Create(5000, Quality.High), Bitrate.Create(0, Quality.MediumHighest), Bitrate.Create(2500, Quality.Medium), Bitrate.Create(1500, Quality.Low), Bitrate.Create(600, Quality.VeryLow) };


        foreach (var Bitrate in Bitrates)
        {
            string Url = ScrapeBit.String(PageContent, "{\"bitrate\":" + Bitrate.amount + ",\"playUrl\":\"", "\","); // FHD High Bitrate

            if (Url == null)
                continue;
            else return new Player(Url, Bitrate.quality);
        }
        return new Player(null, Quality.NotFound);
    }

}
public class Player
{
    public Player(string url, Quality quality)
    {
        Url = url;
        Quality = quality;
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