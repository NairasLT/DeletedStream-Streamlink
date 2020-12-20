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
    public ConfigFile(List<Channel> channels)
    {
        Channels = channels;
    }
    public ConfigFile()
    {
    }

    public bool AutoDelete = false;
    public int AutoDeleteAfterDays = 2;
    public List<FileEntry> DeleteList = new List<FileEntry>();
    public List<Channel> Channels = new List<Channel>();
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
            List<Channel> temp = new List<Channel>();
            temp.Add(new Channel(ConfigExampleText, 6, Platform.YouTube));
            ConfigFile tempfile = new ConfigFile(temp);
            File.WriteAllText(ConfigFile, JsonConvert.SerializeObject(tempfile, Formatting.Indented));
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


public class ActiveChannel
{
    public ActiveChannel(string channelId, TimeSpan timeOut)
    {
        ChannelId = channelId;
        TimeOut = timeOut;
    }

    public string ChannelId { get; set; }
    public TimeSpan TimeOut { get; set; }

    internal async Task Sleep()
    {
        await Task.Delay(TimeOut);
    }

    public async Task Run()
    {
        while (true)
        {
            var Status = await Scrape.GetLivestreamStatusFromChannelId(ChannelId);

            if (!Status.IsLivestreaming)
            {
                await Sleep();
                continue;
            }

            var ytExplode = new YoutubeClient();
            var metadata = await ytExplode.Videos.GetAsync(Status.videoId);
            var StreamObject = new LivestreamObject(metadata, FilePaths.GetThumbnailPath(FileName.Purify($"{metadata.Title} [{DateTime.Now.Ticks.GetHashCode()}].jpeg")), FilePaths.GetLivestreamsPath(FileName.Purify($"{metadata.Title} [{DateTime.Now.Ticks.GetHashCode()}].mp4")));
            try
            {
                new WebClient().DownloadFile(metadata.Thumbnails.MaxResUrl, StreamObject.ThumbnailPath);
            }
            catch (Exception) { }
            await Streamlink.Download(StreamObject.LivestreamPath, metadata.Url);

            var Upload = new Upload(FilePaths.SecretsFile);
            await Upload.Init();
            _ = Upload.CreateWithRetry(StreamObject, TimeSpan.FromHours(3));

            await Sleep();
        }
    }
}
public static class ScrapeBit
{
    public static string String(string SourceText, string ToFindText, string ReadUntilString)
    {
        int dIndex = SourceText.IndexOf(ToFindText); // Get Index in string of the find Text.

        if (dIndex == -1) return null; // If Find Text is not found return.

        SourceText = SourceText.Substring(dIndex + ToFindText.Length);

        int FoundTerminators = 0;
        IList<char> ReadTextChars = new List<char>(); //Where to store the good chars.
        char[] SourceTextChar = SourceText.ToArray(); // To Char Array

        char[] TerminatorChars = ReadUntilString.ToArray(); //To Char Array

        for (int i = 0; i < SourceTextChar.Length; i++)
        {
            if (FoundTerminators == TerminatorChars.Length) break;

            if (SourceText[i] == TerminatorChars[FoundTerminators])
            {
                FoundTerminators++;
                continue;
            }

            FoundTerminators = 0;
            ReadTextChars.Add(SourceText[i]); // Add the good char
        }
        if (ReadTextChars.Count <= 0) return null;
        else return new string(ReadTextChars.ToArray());
    }
}
