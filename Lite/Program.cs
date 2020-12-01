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
            var Filecontent = cfg.Read();

            if (Filecontent != null)
                foreach (var user in Filecontent.Channels)
                {
                    if(user.ChannelId == FilePaths.ConfigExampleText)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine("Please configure the youtube channels, and remove the Example Object.");
                        Console.ResetColor();
                        Console.WriteLine("Press Enter key to Exit.");
                        Console.ReadLine();
                        return;
                    }

                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"Started [{user.ChannelId}] with {user.MinutesTimeOut} minute Timeout.");
                    Console.ResetColor();
                    var Runtime = new ActiveChannel(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                    _ = Runtime.Run();
                }

            Console.ReadLine();
        }
    }
}
