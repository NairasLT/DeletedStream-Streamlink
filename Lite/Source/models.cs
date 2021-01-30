using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using System.Linq;

public class Channel
{
    public Channel(string channelId, int minutesTimeOut, Platform platform)
    {
        ChannelId = channelId;
        MinutesTimeOut = minutesTimeOut;
        Platform = platform;
    }

    public string ChannelId { get; set; }
    public int MinutesTimeOut { get; set; }
    public Platform Platform { get; set; }
}

public enum Platform
{
    YouTube,
    Trovo
}

class ConfigFile
{
    public ChannelGroup[] ChannelGroups;

    public ConfigFile(ChannelGroup[] channelGroups)
    {
        ChannelGroups = channelGroups;
    }
}

public class ChannelGroup
{
    public IList<Channel> Channels = new List<Channel>();
    public bool Async = false;
    public int AsyncMinuteDelay = 6;

    public ChannelGroup(IList<Channel> channels, bool async, int asyncMinuteDelay)
    {
        Channels = channels;
        Async = async;
        AsyncMinuteDelay = asyncMinuteDelay;
    }


    public static void AsynchronousInfiniteStart(ChannelGroup group)
    {
        foreach (var user in group.Channels)
        {
            if (user.ChannelId == FilePaths.ConfigExampleText) { CError.ErrorExampleObjectFound(); return; }

            CMessage.InctanceStarted(user, true);


            switch (user.Platform)
            {
                case Platform.Trovo:
                    var trovo = new TrovoStreamer(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                    _ = trovo.RunInfinite();
                    break;

                case Platform.YouTube:
                    var Runtime = new ActiveChannel(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                    _ = Runtime.RunInfinite();
                    break;
            }
        }
    }


    public async static Task SynchronousInfinite(ChannelGroup group)
    {
        while (true)
        {
            foreach (var user in group.Channels)
            {
                if (user.ChannelId == FilePaths.ConfigExampleText) { CError.ErrorExampleObjectFound(); return;}

                CMessage.InctanceStarted(user, false);

                switch (user.Platform)
                {

                    case Platform.Trovo:
                        var trovo = new TrovoStreamer(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                        await trovo.RunOnce();
                        break;

                    case Platform.YouTube:
                        var Runtime = new ActiveChannel(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                        await Runtime.RunOnce();
                        break;
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(group.AsyncMinuteDelay));
        }
    }

}



public class FileName
{
    public static string Purify(string Input)
    {
        return Input.Replace('*', ' ').Replace('<', ' ').Replace('>', ' ').Replace(':', ' ').Replace('\"', ' ').Replace('\\', ' ').Replace('/', ' ').Replace('|', ' ').Replace('?', ' ');
    }
}

public class FilePaths
{
    private static string ThumbnailFolder = $"{Directory.GetCurrentDirectory()}\\Thumbnails\\";
    private static string LivestreamsFolder = $"{Directory.GetCurrentDirectory()}\\Livestreams\\";
    private static string SecretsFolder = $"{Directory.GetCurrentDirectory()}\\Secrets\\";
    private static string ConfigFolder = $"{Directory.GetCurrentDirectory()}\\Config\\";
    public static string SecretsFileDefaultText = "Replace this file With your YouTube Api Oauth secrets file.";
    public static string ConfigFile = $"{ConfigFolder}Config.json";
    public static string SecretsFile = $"{SecretsFolder}client_secrets.json";
    public static string ConfigExampleText = "Enter Here YouTube Channel Id Example: UCSIKKd_AkoV3c4F5CI4JAnQ";
    public static void Setup()
    {
        CheckFilesystem(new string[] { ThumbnailFolder, LivestreamsFolder, SecretsFolder, ConfigFolder }, true);
        CheckFile(SecretsFile, SecretsFileDefaultText);
        CheckConfigFile();

        IsExistStreamLink();
        IsCheckSecretsFile();
    }
    public static bool IsCheckSecretsFile()
    {
        string SecretsFileText = File.ReadAllText(FilePaths.SecretsFile);
        if (SecretsFileText != FilePaths.SecretsFileDefaultText || SecretsFileText.Length > FilePaths.SecretsFileDefaultText.Length)
        {
            new Upload(FilePaths.SecretsFile).Init().Wait();
            return true;
        }
        else
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("YouTube API, client_secrets.json File is empty or incorrect! Press any key to Exit.");
            Console.ResetColor();
            Console.ReadLine();
            Environment.Exit(0);
            return false;
        }
    }
    public static bool IsExistStreamLink()
    {
        if (!File.Exists("C:\\Program Files (x86)\\Streamlink\\bin\\streamlink.exe"))
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("Streamlink is not installed in the default path, it should be in C:\\Program Files (x86)\\Streamlink\\bin\\streamlink.exe");
            Console.ResetColor();
            Console.ReadLine();
            Environment.Exit(0);
            return false;
        }
        else return true;
    }
    private static void CheckFilesystem(string[] Paths, bool IsDirectory)
    {
        if (IsDirectory)
            foreach (var Path in Paths)
                if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);

        if (!IsDirectory)
            foreach (var Path in Paths)
                if (!File.Exists(Path)) File.Create(Path);
    }

    private static void CheckConfigFile()
    {
        if (!File.Exists(ConfigFile))
        {
            IList<ChannelGroup> Groups = new List<ChannelGroup>();

            IList<Channel> ChannelsForTemp = new List<Channel>();
            ChannelsForTemp.Add(new Channel(ConfigExampleText, 6, Platform.YouTube));


            ChannelGroup TempGroup = new ChannelGroup(ChannelsForTemp, false, 6);
            Groups.Add(TempGroup);

            ConfigFile configFile = new ConfigFile(Groups.ToArray());

            File.WriteAllText(ConfigFile, JsonConvert.SerializeObject(configFile, Formatting.Indented));
        }
    }
    private static void CheckFile(string Path, string Content)
    {
        if (!File.Exists(Path))
            File.WriteAllText(SecretsFile, Content);
    }
    public static string GetThumbnailPath(string Filename)
    {
        return $"{ThumbnailFolder}{Filename}";
    }

    public static string GetLivestreamsPath(string Filename)
    {
        return $"{LivestreamsFolder}{Filename}";
    }
}
public class LivestreamObject
{
    public LivestreamObject(Video info, string thumbnailPath, string livestreamPath)
    {
        Info = info;
        ThumbnailPath = thumbnailPath;
        LivestreamPath = livestreamPath;
    }
    public LivestreamObject(string livestreamPath)
    {
        LivestreamPath = livestreamPath;
    }

    public Video Info { get; set; }
    public string ThumbnailPath { get; set; }
    public string LivestreamPath { get; set; }
}



