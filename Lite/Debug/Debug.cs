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

    public static void ErrorInRunBlock(string API)
    {
        ConsoleColor.Red.WriteLine($"Exception occurred in {API} Run() function");
    }


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
    public static void ErrorCheckingUpdates(Exception x)
    {
        ConsoleColor.DarkGreen.WriteLine($"Error occured while checking for updates. Please check https://github.com/Nojus0/YouTube-Livestream-Archiver, more details {x.Message}");
    }
    public static void ErrorVersionFile(Exception x)
    {
        ConsoleColor.DarkGreen.WriteLine($"Error occurred reading Version file ( Latest.json ) more details: {x.Message}");
    }
    public static void TrovoGqlJsonParseError(Exception x)
    {
        ConsoleColor.DarkGreen.WriteLine($"Parsing error occured while trying to parse json Response from TrovoGql Api. more details: {x.Message}");
    }

}
public static class CMessage
{
    public static void InctanceStarted(Channel user, bool Async)
    {
        if (Async)
            ConsoleColor.DarkGreen.WriteLine($"[ASYNC]Started thread for [{user.ChannelId}] delay {user.MinutesTimeOut} min Platform {user.Platform}");
        else
            ConsoleColor.DarkGreen.WriteLine($"[SYNC] Checking [{user.ChannelId}] delay {user.MinutesTimeOut} min Platform {user.Platform}");
    }
    public static void LivestreamFound(string Title, string Quality, Platform platform)
    {
        ConsoleColor.DarkGreen.WriteLine($"Found {platform} Livestream with title {Title} with quality {Quality}.");
    }
    public static void LivestreamFound(string Title, Platform platform)
    {
        ConsoleColor.DarkGreen.WriteLine($"Found {platform} Livestream with title {Title}.");
    }
    public static void GotResponseScheduledLivestream(string User, string API)
    {
        ConsoleColor.DarkGreen.WriteLine($"[{User}] Got response from {API}, livestream is Scheduled.");
    }

    public static void GotResponseFromAPIStreamerOffline(string User, string API)
    {
        ConsoleColor.DarkGreen.WriteLine($"[{User}] Got response from {API}, streamer is Offline.");
    }
    public static void GotResponseNonExistentUser(string User, string API)
    {
        ConsoleColor.DarkYellow.WriteLine($"[{User}] Got response from {API}, streamer does not exist.");
    }

}