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
            var client = new RestClient("https://raw.githubusercontent.com/Nojus0/YouTube-Livestream-Archiver/master/Lite/Latest.json");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteAsync(request);
            UpdateInformation WebVersion = JsonConvert.DeserializeObject<UpdateInformation>(response.Content);

            if(CurrentInformation.LatestVersion < WebVersion.LatestVersion)
            { // Implement more
                switch (WebVersion.Severity)
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
                ConsoleColor.DarkRed.WriteLine(WebVersion.Title);
                ConsoleColor.Red.WriteLine(WebVersion.Description);
                ConsoleColor.Red.WriteLine($"Version {WebVersion.LatestVersion}");
            }

        }
        catch(Exception x) { CError.ErrorCheckingUpdates(x); }
    }
}


public class UpdateInformation
{
    [JsonProperty("Title")]
    public string Title { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("LatestVersion")]
    public long LatestVersion { get; set; }

    [JsonProperty("Severity")]
    public string Severity { get; set; }
}

