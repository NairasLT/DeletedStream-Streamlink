using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
class Upload
{
    YouTubeService service;
    string Secrets = string.Empty;
    string User = string.Empty;

    /// <summary>
    /// MUST CALL CALL 'INIT' FUNCTION BEFORE USING!!
    /// </summary>
    /// <param name="ClientSecretsPath"></param>
    public Upload(string ClientSecretsPath, string SpecifiedUser = "user")
    {
        Secrets = ClientSecretsPath;
        User = SpecifiedUser;
    }
    public async Task Init() //Using this function because you cannot make a constructor async.
    {
        service = await GetService(Secrets, User);
    }

    public async Task<bool> Create(LivestreamObject livestream)
    {
        try
        {
            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = livestream.Info.Title;
            video.Snippet.Description = livestream.Info.Description;
            video.Snippet.Tags = new string[] { "" };
            video.Snippet.CategoryId = "22";
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "unlisted";
            var filePath = livestream.LivestreamPath;

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = service.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ResponseReceived += async (Video) =>
                {
                    await SetThumbnail(Video.Id, livestream.ThumbnailPath);
                };
                await videosInsertRequest.UploadAsync();
                fileStream.Dispose();
            }

            return true;
        }
        catch (Exception) {  return false; }
    }

    public async Task CreateWithRetry(LivestreamObject livestream, TimeSpan RetryTimeout)
    {
        try
        {
            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = livestream.Info.Title;
            video.Snippet.Description = livestream.Info.Description;
            video.Snippet.Tags = new string[] { "" };
            video.Snippet.CategoryId = "22";
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "private";
            var filePath = livestream.LivestreamPath;

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = service.Videos.Insert(video, "snippet,status", fileStream, "video/*");

                videosInsertRequest.ResponseReceived += async (Video) =>
                {
                    if (File.Exists(livestream.ThumbnailPath))
                        await SetThumbnail(Video.Id, livestream.ThumbnailPath);
                };
                await videosInsertRequest.UploadAsync();
                Console.WriteLine($"Uploaded Video {video.Id}");
                fileStream.Dispose();
            }
        }
        catch (Exception x)
        {
            Console.WriteLine($"Error Uploaded most likely exeeded quota limit {x.Message}   Retrying...");
            await Task.Delay(RetryTimeout);
            _ = CreateWithRetry(livestream, RetryTimeout);
        }
    }
    public async Task CreateWithRetry(string Title, string Description, string Path, TimeSpan timeout)
    {
        try
        {
            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = Title;
            video.Snippet.Description = Description;
            video.Snippet.Tags = new string[] { "" };
            video.Snippet.CategoryId = "22";
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "private";
            var filePath = Path;

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = service.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                await videosInsertRequest.UploadAsync();
                Console.WriteLine($"Uploaded Video {video.Id}");
                fileStream.Dispose();
            }
        }
        catch (Exception x)
        {
            Console.WriteLine($"Error Uploaded most likely exeeded quota limit {x.Message}   Retrying...");
            await Task.Delay(timeout);
            _ = CreateWithRetry(Title, Description, Path, timeout);
        }
    }


    public async Task Start(string title, string description, string path)
    {
        var video = new Video();
        video.Snippet = new VideoSnippet();
        video.Snippet.Title = title;
        video.Snippet.Description = description;
        video.Snippet.Tags = new string[] { "" };
        video.Snippet.CategoryId = "22";
        video.Status = new VideoStatus();
        video.Status.PrivacyStatus = "private";
        var filePath = path;
        using (var fileStream = new FileStream(filePath, FileMode.Open))
        {
            var videosInsertRequest = service.Videos.Insert(video, "snippet,status", fileStream, "video/*");
            await videosInsertRequest.UploadAsync();
            fileStream.Dispose();
        }
    }
    /// <summary>
    /// Only Jpeg, files are supported, changing file extension to png or else dosent work you need to save as png!
    /// </summary>
    /// <param name="videoid"></param>
    /// <param name="ThumbnailPath"></param>
    /// <returns></returns>
    public async Task SetThumbnail(string videoid, string ThumbnailPath)
    {
        try
        {
            var fs = File.OpenRead(ThumbnailPath);//{Path.GetExtension(ThumbnailPath).Substring(1)}
            var file = service.Thumbnails.Set(videoid, fs, $"image/jpeg");
            await file.UploadAsync();
            fs.Dispose();
            Console.WriteLine($"Thumbnail set for video: {videoid}");
        }
        catch (Exception) { Console.WriteLine("Error Setting Thumbnail!"); }
    }
    private async Task<YouTubeService> GetService(string SecretsPath, string user)
    {
        UserCredential credential;
        using (var stream = new FileStream(SecretsPath, FileMode.Open, FileAccess.Read))
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, new[] { YouTubeService.Scope.YoutubeUpload }, user, CancellationToken.None);
            stream.Dispose();
        }
        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
        });
        return youtubeService;
    }
}
