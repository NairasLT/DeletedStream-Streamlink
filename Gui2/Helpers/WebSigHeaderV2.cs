using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gui2.Helpers
{
    class WebSigHeaderV2
    {
        public static async Task<JsonDecodeResult> BroadcastIdFromChannelId(string ChannelId)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(60);
                var response = await client.GetAsync($"https://www.youtube.com/embed/live_stream?channel={ChannelId}&pbj=1", HttpCompletionOption.ResponseContentRead);
                string responsestring = await response.Content.ReadAsStringAsync();
                JsonDecodeResult decodeResult = JsonDecode(responsestring, "<script >yt.setConfig(", ");yt.setConfig({");

                if (decodeResult.ResultStatus == Status.Fail)
                    return new JsonDecodeResult(null, Status.Fail);

                ResponseRoot YoutubeResponse = JsonConvert.DeserializeObject<ResponseRoot>(decodeResult.Result);

                return new JsonDecodeResult(YoutubeResponse.VIDEO_ID, Status.Success);
            }
            catch (Exception x) { Console.WriteLine($"Exception Id From Channel {x.Message}"); return new JsonDecodeResult(null, Status.Fail); }

        }

        public static JsonDecodeResult JsonDecode(string HtmlString, string Start, string End)
        {
            int StartIndex = HtmlString.IndexOf(Start);

            if (StartIndex < 1)
                return new JsonDecodeResult(null, Status.Fail);

            HtmlString = HtmlString.Substring(StartIndex + Start.Length); //Make that starts from the index.

            var EndIndex = Regex.Match(HtmlString, Regex.Escape(End)).Index;

            HtmlString = HtmlString.Substring(0, EndIndex);
            return new JsonDecodeResult(HtmlString, Status.Success);
        }

    }
}
