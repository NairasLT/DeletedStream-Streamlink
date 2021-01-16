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
            if (group.Async) ChannelGroup.AsynchronousInfiniteStart(group); // Starts thread for all group channels
            else _ = ChannelGroup.SynchronousInfinite(group); // Creates 1 thread for all group channels
        }

    }
}



public static class ConsoleExtension
{
    public static void WriteLine(this ConsoleColor Color, string Text)
    {
        Console.BackgroundColor = Color;
        Console.WriteLine(Text);
        Console.ResetColor();
    }
}

public static class CError
{

    public static void ErrorExampleObjectFound()
    {
        ConsoleColor.Red.WriteLine("Please configure the config file correctly. And remove the example Object");
        ConsoleColor.Red.WriteLine("Please exit.");
    }

}

public static class CMessage
{
    public static void InctanceStarted(Channel user, bool Async)
    {
        if (Async)
            ConsoleColor.Green.WriteLine($"[ASYNC]Started thread for [{user.ChannelId}] delay {user.MinutesTimeOut} min Platform {user.Platform}");
        else
            ConsoleColor.Green.WriteLine($"[SYNC] Checking [{user.ChannelId}] delay {user.MinutesTimeOut} min Platform {user.Platform}");

    }
}

