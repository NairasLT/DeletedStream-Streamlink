using System;
using Newtonsoft.Json;

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
                ConsoleHelpers.WriteInColor("Please configure the config file correctly. And remove the example Object", ConsoleColor.Red);
                ConsoleHelpers.WriteInColor("Press Enter to Exit.", ConsoleColor.Yellow);
                return;
            }

            ConsoleHelpers.WriteInColor($"Started checking [{user.ChannelId}] delay {user.MinutesTimeOut} min Platform {user.Platform}", ConsoleColor.Yellow);
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
    public static void WriteInColor(string Text,  ConsoleColor color)
    {
        Console.BackgroundColor = color;
        Console.WriteLine(Text);
        Console.ResetColor();
    }
}

