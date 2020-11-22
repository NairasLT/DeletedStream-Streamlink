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

        string videoId = ScraperBitString(response.Content, "\\\"videoId\\\":\\\"", "\\\"", 40);
        if (videoId == null) return new LivestreamStatus(false, null);
        else return new LivestreamStatus(true, videoId);
    }


    public static string ScraperBitString(string SourceText, string ToFindText, string ReadUntilString, int SubstringEndIndexOffset)
    {

        int dIndex = SourceText.IndexOf(ToFindText); // Get Index in string of the find Text.

        if (dIndex == -1) return null; // If Find Text is not found return.

        SourceText = SourceText.Substring(dIndex + ToFindText.Length, dIndex + SubstringEndIndexOffset);

        int FoundTerminators = 0; // ? 0 index based
        List<char> ReadTextChars = new List<char>(); //Where to store the good chars.

        char[] SourceTextChar = SourceText.ToArray(); // To Char Array


        char[] TerminatorChars = ReadUntilString.ToArray(); //To Char Array

        for (int i = 0; i < SourceTextChar.Length; i++)
        {

            if (FoundTerminators == TerminatorChars.Length) break;


            if (SourceText[i] == TerminatorChars[FoundTerminators])
            {
                FoundTerminators++;
                continue;
            }


            FoundTerminators = 0;
            ReadTextChars.Add(SourceText[i]); // Add the good char
        }
        Console.WriteLine("Finished");
        if (ReadTextChars.Count <= 0) return null;
        else return new string(ReadTextChars.ToArray());
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
