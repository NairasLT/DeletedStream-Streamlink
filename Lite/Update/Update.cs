using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Update
{

    public UpdateInformation CurrentInformation { get; set; }

    public Update()
    {
        try
        {
            CurrentInformation = new Config<UpdateInformation>(FilePaths.VersionFile).Read();
        }
        catch (Exception x) { CError.ErrorVersionFile(x); }
    }
    
    public async Task Get()
    {
        try
        {
            var client = new RestClient("https://github.com/Nojus0/YouTube-Livestream-Archiver/blob/master/Latest.txt");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var update = JsonConvert.DeserializeObject<UpdateInformation>(response.Content);

            if(CurrentInformation.LatestVersion < update.LatestVersion)
            { // Implement more
                switch (update.Severity)
                {
                    case "Critical":
                        ConsoleColor.Red.WriteLine($"Severity: CRITICAL");
                        break;
                    case "High":
                        ConsoleColor.Red.WriteLine($"Severity: HIGH");
                        break;
                    case "Normal":
                        ConsoleColor.DarkGreen.WriteLine($"Severity: NORMAL");
                        break;
                    case "Low":
                        ConsoleColor.Yellow.WriteLine($"Severity: LOW");
                        break;
                    case "VeryLow":
                        ConsoleColor.Yellow.WriteLine($"Severity: VERYLOW");
                        break;
                    case "Needed":
                        ConsoleColor.Red.WriteLine($"Severity: NEEDED FIX");
                        break;
                }
                ConsoleColor.DarkRed.WriteLine(update.Title);
                ConsoleColor.Red.WriteLine(update.Description);
                ConsoleColor.Red.WriteLine($"Version {update.LatestVersion}");
            }

        }
        catch(Exception) { CError.ErrorCheckingUpdates(); }
    }
}


class UpdateInformation
{
    public string Title { get; set; }
    public string Description { get; set; }
    public double LatestVersion { get; set; }
    public string Severity { get; set; }
}

