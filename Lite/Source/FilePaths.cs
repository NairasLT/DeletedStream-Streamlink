using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FilePaths
{
    private static string ThumbnailFolder = $"{Directory.GetCurrentDirectory()}\\Thumbnails\\";
    private static string LivestreamsFolder = $"{Directory.GetCurrentDirectory()}\\Livestreams\\";
    private static string SecretsFolder = $"{Directory.GetCurrentDirectory()}\\Secrets\\";
    private static string ConfigFolder = $"{Directory.GetCurrentDirectory()}\\Config\\";
    public static string SecretsFileDefaultText = "Replace this file With your YouTube Api Oauth secrets file.";
    public static string VersionFile = $"{Directory.GetCurrentDirectory()}\\Latest.txt";
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
            new Youtube(FilePaths.SecretsFile).Init().Wait();
            return true;
        }
        else
        {
            ConsoleColor.Red.WriteLine("YouTube API, client_secrets.json File is empty or incorrect! Press any key to Exit.");
            Console.ReadLine();
            Environment.Exit(0);
            return false;
        }
    }
    public static bool IsExistStreamLink()
    {
        if (!File.Exists("C:\\Program Files (x86)\\Streamlink\\bin\\streamlink.exe"))
        {
            ConsoleColor.Red.WriteLine("Streamlink is not installed in the default path, it should be in C:\\Program Files (x86)\\Streamlink\\bin\\streamlink.exe");
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

public class FileName
{
    public static string Purify(string Input)
    {
        return Input.Replace("*", "").Replace("<", "").Replace(">", "").Replace(":", "").Replace("\"", "").Replace("\\", "").Replace("/", "").Replace("|", "").Replace("?", "");
    }
}
