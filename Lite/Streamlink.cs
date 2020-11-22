using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lite
{
    class Streamlink
    {
        public static async Task Download(string FilePath, string Url)
        {
            var streamlink = new ProcessStartInfo
            {
                FileName = $"C:\\Program Files (x86)\\Streamlink\\bin\\streamlink.exe",
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Arguments = $" --hls-live-restart -o \"{FilePath}\" \"{Url}\" best",
                UseShellExecute = false,
                RedirectStandardOutput = false,
                CreateNoWindow = false
            };
            Process process = new Process();
            process.StartInfo = streamlink;
            process.Start();
            await Task.Run(() => process.WaitForExit());
        }
    }
}
