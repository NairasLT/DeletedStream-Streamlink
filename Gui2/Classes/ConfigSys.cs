using Gui2.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Gui2.Helpers.ConfigHelper;
namespace Gui2.Classes
{
    class ConfigSys
    {
        string ConfigPath = string.Empty;
        public ConfigSys(string ConfigurationPath)
        {
            ConfigPath = ConfigurationPath;
        }

        /// <summary>
        /// Creates the config file.
        /// </summary>
        public void Create()
        {
            if (!File.Exists(ConfigPath))
                File.WriteAllText(ConfigPath, null);

        }

        /// <summary>
        /// Resets the config file to an empty file.
        /// </summary>
        public void Reset()
        {
            if (File.Exists(ConfigPath))
                File.WriteAllText(ConfigPath, null);
        }

        /// <summary>
        /// Deletes the config file.
        /// </summary>
        public void Delete()
        {
            if (File.Exists(ConfigPath))
                File.Delete(ConfigPath);
        }
        public SaveInfo ReadInfo()
        {
            try
            {
                return JsonConvert.DeserializeObject<SaveInfo>(File.ReadAllText(ConfigPath));
            }
            catch (Exception x) { MessageBox.Show($"Exception occurred: {x.Message}"); return new SaveInfo(); }
        }


        public bool SetSavePath(string Path)
        {
            try
            {
                var current = ReadInfo();
                current.settings.Streams_Save_Folder = Path;
                string Serialize = JsonConvert.SerializeObject(current);
                File.WriteAllText(ConfigPath, Serialize);
                return true;
            }
            catch(Exception) { return false; }
        }


        public bool SetSecretsFilename(string Name)
        {
            try
            {
                var current = ReadInfo();
                current.settings.Client_Secrets = Name;
                string Serialize = JsonConvert.SerializeObject(current);
                File.WriteAllText(ConfigPath, Serialize);
                return true;
            }
            catch (Exception) { return false; }
        }
        public bool AddStreamer(Streamer strm)
        {
            string CurrentCfg = File.ReadAllText(ConfigPath);

            if(IsEmpty(ConfigPath) || !File.Exists(ConfigPath))
            {
                SaveInfo siv = new SaveInfo();
                siv.Streamers.Add(strm);
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(siv));

                return true;
            }
            else
            {
                SaveInfo cfg = JsonConvert.DeserializeObject<SaveInfo>(CurrentCfg);

                foreach(var obj in cfg.Streamers) //Check if already added
                    if(obj.Website == strm.Website)
                    {
                        MessageBox.Show("Streamer already Added");
                        return false;
                    }

                cfg.Streamers.Add(strm);
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(cfg));
                return true;
            }
        }

        /// <summary>
        /// Removes the channel id and writes to Config file.
        /// </summary>
        /// <param name="Id"></param>
        public void RemoveChannelId(string Id)
        {
            if (IsEmpty(ConfigPath))
            {
                MessageBox.Show("You tried removing an a id from an empty file");
                return;
            }

            string Config = File.ReadAllText(ConfigPath);

            SaveInfo cfg = JsonConvert.DeserializeObject<SaveInfo>(Config);

            foreach (Streamer IdObj in cfg.Streamers.ToArray())
                if (IdObj.Website == Id)
                    cfg.Streamers.Remove(IdObj);

            string Serialize = JsonConvert.SerializeObject(cfg);
            File.WriteAllText(ConfigPath, Serialize);
        }



    }
}
