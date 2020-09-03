using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YoutubeExplode.Videos;
using static System.Environment;

namespace Gui2.Helpers
{
    class ConfigHelper
    {
        public static string LocalFolder = $"{Environment.GetFolderPath(SpecialFolder.Desktop)}\\StreamSave\\";
        public static string ThumbnailsFolder = $"{LocalFolder}Thumbnails\\";
        public static string StreamsFolder = $"{LocalFolder}Streams\\";
        public static string ClientSecretsFolder = $"{LocalFolder}Secrets\\";
        public static string StreamManifestFolder = $"{LocalFolder}StreamManifests\\";
        public static string LocalData = $"{LocalFolder}data.json";
        public static string StreamlinkFile = $"\"C:\\Program Files (x86)\\Streamlink\\bin\\streamlink.exe\"";
        private static void FileSetup(string Path, bool IsDirectory)
        {
            try
            {
                if (IsDirectory)
                    if (!Directory.Exists(Path))
                        Directory.CreateDirectory(Path);
                if (!IsDirectory)
                    if (!File.Exists(Path))
                        File.WriteAllText(Path, "");
            }
            catch (Exception x) { MessageBox.Show("Exeception Occured: " + x); }
        }


        private static void JsonFileSetup(string Path, object Type)
        {
            try
            {
                if (!File.Exists(Path))
                    File.WriteAllText(Path, JsonConvert.SerializeObject(Type, Formatting.Indented));
            }
            catch (Exception x) { MessageBox.Show("Exeception Occured: " + x); }
        }

        public static void LocalFilesSetup()
        {
            FileSetup(LocalFolder, true);
            FileSetup(ThumbnailsFolder, true);
            FileSetup(ClientSecretsFolder, true);
            FileSetup(StreamsFolder, true);
            FileSetup(StreamManifestFolder, true);
            JsonFileSetup(LocalData, new SaveInfo());
        }

        public static bool IsEmpty(string Path)
        {
            string Content = File.ReadAllText(Path);
            if (Content.Length < 1) //Empty
                return true;
            else
                return false;
        }
    }
}