using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;

class Program
{
    static void Main(string[] args)
    {
        FilePaths.Setup();
        Console.WriteLine(FilePaths.ConfigFile);
        var cfg = new Config<ConfigFile>(FilePaths.ConfigFile);
        var Filecontent = cfg.Read();

        if (Filecontent == null)
            return;

        foreach (var user in Filecontent.Channels)
        {
            if (user.ChannelId == FilePaths.ConfigExampleText) // TODO: Add Class for the console text.
            {
                ConsoleHelpers.ShowConfigureChannels();
                return;
            }

            ConsoleHelpers.ShowStartedChannel(user);


            switch (user.Platform)
            {
                case Platform.YouTube:
                    var Runtime = new ActiveChannel(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                    _ = Runtime.Run();
                    break;

                case Platform.Trovo:
                    var trovo = new TrovoStreamer(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                    _ = trovo.BeginLoop();
                    break;
            }
        }

        while (true)
            Console.ReadLine();
    }
}

public static class ConsoleHelpers
{
    public static void ShowConfigureChannels()
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine("Please configure the Streamer Channels, and remove the Example Object.");
        Console.ResetColor();
        Console.WriteLine("Press Enter key to Exit.");
        Console.ReadLine();
    }

    public static void ShowStartedChannel(Channel user)
    {
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine($"Started [{user.ChannelId}] with {user.MinutesTimeOut} minute Timeout.");
        Console.ResetColor();
    }

}

