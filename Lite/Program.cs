using System;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        Startup.Launch();

        while (true)
            Console.ReadLine();
    }
}

public class Startup
{
    public static void Launch()
    {
        FilePaths.Setup();
        Console.WriteLine(FilePaths.ConfigFile);
        var cfg = new Config<ConfigFile>(FilePaths.ConfigFile);
        var Filecontent = cfg.Read();

        if (Filecontent == null) return;

        foreach (var group in Filecontent.ChannelGroups)
        {
            if (group.Async)
                PluginHandler.Async(group); // Starts thread for all group channels
            else
                _ = PluginHandler.Sync(group); // Creates 1 thread for all group channels
        }

    }
}

