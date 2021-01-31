using System;

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
    public static void TryUploadFileNonExistFile()
    {
        ConsoleColor.DarkYellow.WriteLine($"Error handled: tried to upload non exsisting file to YouTube. Skipping..");
    }
    public static void YouTubeAPIDailyQuotaLimitReached(Exception x)
    {
        ConsoleColor.DarkGreen.WriteLine($"Error uploading video to YouTube, most likely you have exceeded your YouTube API Daily Limit, 3-4 Videos per day, Trying to upload again after 3 hours. Exception Details: {x.Message}");
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