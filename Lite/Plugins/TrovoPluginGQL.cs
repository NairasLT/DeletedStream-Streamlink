using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

class TrovoPluginGQL : IPlugin // Soon to be implmented using thier GraphQl Api. Old one will be depracated.
{
    public string Name { get; set; }
    public TimeSpan Timeout { get; set; }
    private string API_NAME = "Trovo API";

    public TrovoPluginGQL(string name, TimeSpan timeout)
    {
        Name = name;
        Timeout = timeout;
    }

    public async Task Run()
    {
        GetLiveInfo info = await GetInfoForSpecified();
        if (info == null || info.ProgramInfo == null) { CMessage.GotResponseNonExistentUser(Name, API_NAME); return; }

        if (!info.IsLive) { CMessage.GotResponseFromAPIStreamerOffline(Name, API_NAME); return; }

        string Title = info.ProgramInfo.Title.ToString(); // ToString, not tested, maybe converts \n \3c.. chars like that to proper chars. 
        string Path = FilePaths.GetLivestreamsPath(FileName.Purify($"{info.ProgramInfo.Title} [{DateTime.Now.Ticks.GetHashCode()}].mp4"));
        string Description = info.StreamerInfo.Info.ToString();

        if (info.ProgramInfo.StreamInfo.Length < 1) return;
        StreamInfo HighestQuality = GetValidPlayUrl(info.ProgramInfo.StreamInfo);
        if (HighestQuality == null) return;

        CMessage.LivestreamFound(Title, HighestQuality.Desc, Platform.Trovo);

        await Download(HighestQuality.PlayUrl, Path);

        var Upload = new Youtube(FilePaths.SecretsFile);
        await Upload.Init();
        _ = Upload.UploadWithRetry(new YoutubeUpload() { LivestreamPath = Path, Title = info.ProgramInfo.Title, Description = Description, ThumbnailPath = null }, TimeSpan.FromHours(3));
    }
    private async Task<GetLiveInfo> GetInfoForSpecified()
    {
        var client = new RestClient("https://gql.trovo.live/");
        client.Timeout = -1;
        var request = new RestRequest(Method.POST);
        request.AddHeader("Content-Type", "application/json");
        request.AddParameter("application/json", "[\r\n    {\r\n        \"operationName\": \"getLiveInfo\",\r\n        \"variables\": {\r\n            \"params\": {\r\n                \"userName\": \"" + Name + "\",\r\n                \"requireDecorations\": true\r\n            }\r\n        },\r\n        \"extensions\": {\r\n            \"persistedQuery\": {\r\n                \"version\": 1,\r\n                \"sha256Hash\": \"0fc70a5c9d328f1bdccd9516d309fffbc915dfd6e9283458fc8356e22ded97bc\"\r\n            },\r\n            \"url\": \"/" + Name + "\"\r\n        }\r\n    }\r\n]", ParameterType.RequestBody);
        IRestResponse response = await client.ExecuteAsync(request);
        try
        {
            return TrovoGql.FromJson(response.Content)[0].Data.GetLiveInfo;
        }
        catch (Exception x) { CError.TrovoGqlJsonParseError(x); return null; }
    }
    private StreamInfo GetValidPlayUrl(StreamInfo[] info)
    {
        foreach (StreamInfo strinfo in info)
        {
            if (strinfo.PlayUrl == null || strinfo.PlayUrl == string.Empty) continue;

            else
                return strinfo;
        }
        return null;
    }

    public async Task RunInfinite()
    {
        while (true)
        {
            await Run();
            await Task.Delay(Timeout);
        }
    }
    private async Task Download(string Url, string Path)
    {
        try { await new WebClient().DownloadFileTaskAsync(Url, Path); } catch (Exception x) { Console.WriteLine($"Exception Occured while Downloading Livestream: {x.Message}"); return; }
    }
}


public partial class TrovoGql
{
    [JsonProperty("data")]
    public Data Data { get; set; }
}

public partial class Data
{
    [JsonProperty("getLiveInfo")]
    public GetLiveInfo GetLiveInfo { get; set; }
}

public partial class GetLiveInfo
{
    [JsonProperty("streamerInfo")]
    public StreamerInfo StreamerInfo { get; set; }

    [JsonProperty("programInfo")]
    public ProgramInfo ProgramInfo { get; set; }

    [JsonProperty("categoryInfo")]
    public CategoryInfo CategoryInfo { get; set; }

    [JsonProperty("channelInfo")]
    public ChannelInfo ChannelInfo { get; set; }

    [JsonProperty("isLive")]
    public bool IsLive { get; set; }

    [JsonProperty("decorations")]
    public object[] Decorations { get; set; }

    [JsonProperty("channelControlInfo")]
    public ChannelControlInfo ChannelControlInfo { get; set; }

    [JsonProperty("streamerPrivilegeInfo")]
    public StreamerPrivilegeInfo StreamerPrivilegeInfo { get; set; }

    [JsonProperty("streamerRaidStatusInfo")]
    public object StreamerRaidStatusInfo { get; set; }

    [JsonProperty("streamerHostInfo")]
    public object StreamerHostInfo { get; set; }

    [JsonProperty("__typename")]
    public string Typename { get; set; }
}

public partial class CategoryInfo
{
    [JsonProperty("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("shortName")]
    public string ShortName { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("__typename")]
    public string Typename { get; set; }
}

public partial class ChannelControlInfo
{
    [JsonProperty("notifyStreamerToChangeChannelInfo")]
    public bool NotifyStreamerToChangeChannelInfo { get; set; }

    [JsonProperty("__typename")]
    public string Typename { get; set; }
}

public partial class ChannelInfo
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("viewers")]
    public long Viewers { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("languageName")]
    public string LanguageName { get; set; }

    [JsonProperty("audiType")]
    public string AudiType { get; set; }

    [JsonProperty("__typename")]
    public string Typename { get; set; }
}

public partial class ProgramInfo
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("coverUrl")]
    public Uri CoverUrl { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("streamInfo")]
    public StreamInfo[] StreamInfo { get; set; }

    [JsonProperty("defaultLevelType")]
    public long DefaultLevelType { get; set; }

    [JsonProperty("startTm")]
    public long StartTm { get; set; }

    [JsonProperty("isOpenLiveTimeShift")]
    public bool IsOpenLiveTimeShift { get; set; }

    [JsonProperty("liveTimeShiftDuration")]
    public long LiveTimeShiftDuration { get; set; }

    [JsonProperty("statusTimeShiftUrl")]
    public string StatusTimeShiftUrl { get; set; }

    [JsonProperty("tagList")]
    public object[] TagList { get; set; }

    [JsonProperty("__typename")]
    public string Typename { get; set; }
}

public partial class StreamInfo
{
    [JsonProperty("bitrate")]
    public long Bitrate { get; set; }

    [JsonProperty("playUrl")]
    public string PlayUrl { get; set; }

    [JsonProperty("desc")]
    public string Desc { get; set; }

    [JsonProperty("encodeType")]
    public string EncodeType { get; set; }

    [JsonProperty("levelType")]
    public long LevelType { get; set; }

    [JsonProperty("playTimeShiftUrl")]
    public string PlayTimeShiftUrl { get; set; }

    [JsonProperty("vipOnly")]
    public bool VipOnly { get; set; }

    [JsonProperty("viewerStatus")]
    public string ViewerStatus { get; set; }

    [JsonProperty("__typename")]
    public string Typename { get; set; }
}

public partial class StreamerInfo
{
    [JsonProperty("uid")]
    public long Uid { get; set; }

    [JsonProperty("nickName")]
    public string NickName { get; set; }

    [JsonProperty("faceUrl")]
    public Uri FaceUrl { get; set; }

    [JsonProperty("gender")]
    public string Gender { get; set; }

    [JsonProperty("userName")]
    public string UserName { get; set; }

    [JsonProperty("info")]
    public string Info { get; set; }

    [JsonProperty("subscribeable")]
    public bool Subscribeable { get; set; }

    [JsonProperty("socialLinks")]
    public SocialLinks SocialLinks { get; set; }

    [JsonProperty("__typename")]
    public string Typename { get; set; }
}

public partial class SocialLinks
{
    [JsonProperty("instagram")]
    public Uri Instagram { get; set; }

    [JsonProperty("twitter")]
    public Uri Twitter { get; set; }

    [JsonProperty("facebook")]
    public Uri Facebook { get; set; }

    [JsonProperty("discord")]
    public Uri Discord { get; set; }

    [JsonProperty("youtube")]
    public Uri Youtube { get; set; }

    [JsonProperty("__typename")]
    public string Typename { get; set; }
}

public partial class StreamerPrivilegeInfo
{
    [JsonProperty("StreamerBadgeInfos")]
    public StreamerBadgeInfo[] StreamerBadgeInfos { get; set; }

    [JsonProperty("__typename")]
    public string Typename { get; set; }
}

public partial class StreamerBadgeInfo
{
    [JsonProperty("streamerBadge")]
    public string StreamerBadge { get; set; }

    [JsonProperty("resourceName")]
    public string ResourceName { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("__typename")]
    public string Typename { get; set; }
}

public partial class TrovoGql
{
    public static TrovoGql[] FromJson(string json) => JsonConvert.DeserializeObject<TrovoGql[]>(json, Converter.Settings);
}

public static class Serialize
{
    public static string ToJson(this TrovoGql[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
}

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
    };
}

internal class ParseStringConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        long l;
        if (Int64.TryParse(value, out l))
        {
            return l;
        }
        throw new Exception("Cannot unmarshal type long");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (long)untypedValue;
        serializer.Serialize(writer, value.ToString());
        return;
    }

    public static readonly ParseStringConverter Singleton = new ParseStringConverter();
}