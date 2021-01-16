using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
class Scrape
{
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
    public class LivestreamStatus
    {
        public LivestreamStatus(bool IsLive, string VideoId)
        {
            IsLivestreaming = IsLive;
            videoId = VideoId;
        }

        public bool IsLivestreaming { get; set; }
        public string videoId { get; set; }

    }

}
