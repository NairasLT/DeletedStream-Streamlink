using Gui2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Gui2.Classes
{
    public class CurrentlyStreaming
    {
        List<Streamer> Streamers = new List<Streamer>();
        public ListBox List;


        public CurrentlyStreaming(ListBox Display)
        {
            List = Display;
        }

        public void RemoveStreamer(Streamer streamer)
        {
            foreach (var Streamer in Streamers.ToArray())
                if (Streamer == streamer)
                {
                    Streamers.Remove(Streamer);
                    List.Items.Remove($"{Streamer.Website} : {streamer.platform}");
                }
        }

        public void AddStreamer(Streamer streamer)
        {
            Streamers.Add(streamer);
            List.Items.Add($"{streamer.Website} : {streamer.platform}");
        }

    }
}
