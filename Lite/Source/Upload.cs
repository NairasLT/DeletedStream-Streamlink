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
class Youtube
{
    YouTubeService service;
    string Secrets = string.Empty;
    string User = string.Empty;
    /// <summary>
    /// MUST CALL CALL 'INIT' FUNCTION BEFORE USING!!
    /// </summary>
    /// <param name="ClientSecretsPath"></param>
    public Youtube(string ClientSecretsPath, string SpecifiedUser = "user")
    {
        Secrets = ClientSecretsPath;
        User = SpecifiedUser;
    }
    public async Task Init() //Using this function because you cannot make a constructor async.
    {
        service = await GetService(Secrets, User);
    }
    public async Task UploadWithRetry(YoutubeUpload info, TimeSpan RetryTimeout)
    {
        try
        {
            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = info.Title;
            video.Snippet.Description = info.Description;
            video.Snippet.Tags = new string[] { "" };
            video.Snippet.CategoryId = "22";
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "private";
            var filePath = info.LivestreamPath;

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = service.Videos.Insert(video, "snippet,status", fileStream, "video/*");

                videosInsertRequest.ResponseReceived += async (Video) =>
                {
                    if (info.ThumbnailPath != null && File.Exists(info.ThumbnailPath))
                    {
                        await SetThumbnail(Video.Id, info.ThumbnailPath);
                        Console.WriteLine($"UPLOADED VIDEO WITH ID {video.Id} TITLE: {info.Title} PATH: {info.LivestreamPath}");
                    }
                };
                await videosInsertRequest.UploadAsync();
                fileStream.Dispose();
            }
        }
        catch(FileNotFoundException)
        {
            CError.TryUploadFileNonExistFile();
        }
        catch (Exception x)
        {
            CError.YouTubeAPIDailyQuotaLimitReached(x);
            await Task.Delay(RetryTimeout);
            _ = UploadWithRetry(info, RetryTimeout);
        }
    }
    /// <summary>
    /// Only Jpeg files are supported.
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


/// <summary>
/// Used for Youtube class, provides upload information, paths, titles....
/// </summary>
public class YoutubeUpload
{
    public YoutubeUpload(string title, string description, string thumbnailPath, string livestreamPath)
    {
        Title = title;
        Description = description;
        ThumbnailPath = thumbnailPath;
        LivestreamPath = livestreamPath;
    }
    public YoutubeUpload() { }

    public string Title { get; set; }
    public string Description { get; set; }
    public string ThumbnailPath { get; set; }
    public string LivestreamPath { get; set; }
}

