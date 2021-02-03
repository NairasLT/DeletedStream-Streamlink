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
        _ = new Update().Get();
        var cfg = new Config<ConfigFile>(FilePaths.ConfigFile);
        var Filecontent = cfg.Read();

        if (Filecontent == null) return;

        foreach (var group in Filecontent.ChannelGroups)
        {
            if (group.Async)
                ChannelGroup.AsynchronousInfiniteStart(group); // Starts thread for all group channels
            else
                _ = ChannelGroup.SynchronousInfinite(group); // Creates 1 thread for all group channels
        }

    }
}

