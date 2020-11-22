using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;

namespace Lite
{
    class Program
    {
        static void Main(string[] args)
        {

            FilePaths.Setup();

            Console.WriteLine(FilePaths.ConfigFile);

            var cfg = new Config<ConfigFile>(FilePaths.ConfigFile);
            cfg.Create(new ConfigFile()); //how to access the T char in config class so i dont need to do this?
            new Upload(FilePaths.SecretsFile).Init().Wait();

            var Filecontent = cfg.Read();

            if (Filecontent != null)
                foreach (var user in Filecontent.Channels)
                {
                    Console.WriteLine($"Started {user.ChannelId} with {user.MinutesTimeOut} minute Timeout.");
                    var Runtime = new ActiveChannel(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                    _ = Runtime.Run();
                }

            Console.ReadLine();
        }
    }
}
