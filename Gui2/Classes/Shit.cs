using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Gui2.Classes
{
    class Shit
    {
        public class CurrentlyStreaming
        {
            List<Streamer> Streamers = new List<Streamer>();
            public ListBox List;

            CurrentlyStreaming(ListBox Display)
            {
                List = Display;
            }

            public void RemoveStreamer(string ChannelUrl)
            {
                foreach (var Streamer in Streamers)
                    if (Streamer.ChannelUrl == ChannelUrl)
                    {
                        Streamers.Remove(Streamer);
                        List.Items.Remove(Streamer.ChannelName);
                    }
            }

            public void AddStreamer(Streamer streamer)
            {
                Streamers.Add(streamer);
                List.Items.Add(streamer.ChannelName);
            }

        }
        public struct Streamer
        {
            public string ChannelName;
            public string ChannelUrl;
            public string ChannelId;
        }



        /*            Signature VideoIdSignature;
            VideoIdSignature.CharTerminator = '\"';
            VideoIdSignature.MemberToAccess = "\"videoId\":\""; //Not from videos tab, other channels have different layout
            VideoIdSignature.signature = "{\"style\":\"BADGE_STYLE_TYPE_LIVE_NOW\"";
            VideoIdSignature.Webpage = ChannelVideosUrl;*/
    }
}
