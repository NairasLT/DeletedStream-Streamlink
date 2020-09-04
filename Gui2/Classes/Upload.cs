using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Gui2.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Gui2.Helpers.ConfigHelper;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gui2.Classes
{
    class Upload
    {
        /// <summary>
        /// PLEASE CALL 'INIT' FUNCTION BEFORE USING!!
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

        YouTubeService service;
        string Secrets = string.Empty;
        string User = string.Empty;

        public async Task Start(SavedVideo s)
        {
            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = s.YouTubeManifest.Title;
            video.Snippet.Description = s.YouTubeManifest.Description;
            video.Snippet.Tags = new string[] {""};
            video.Snippet.CategoryId = "22";
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "unlisted";
            var filePath = s.Files.VideoPath;

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = service.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ResponseReceived += async (Video) =>
                {
                    await SetThumbnail(Video.Id,s.Files.ThumbnailPath);
                };
                await videosInsertRequest.UploadAsync();
                fileStream.Dispose();
            }
        }
        public async Task Start(string Title, string Desctiption, string VideoPath)
        {
            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title =Title;
            video.Snippet.Description = Desctiption;
            video.Snippet.Tags = new string[] { "" };
            video.Snippet.CategoryId = "22";
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "unlisted";
            var filePath = VideoPath;
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
        public async Task SetThumbnail(string videoid,string ThumbnailPath)
        {
            try
            {
                var fs = File.OpenRead(ThumbnailPath);//{Path.GetExtension(ThumbnailPath).Substring(1)}
                var file = service.Thumbnails.Set(videoid, fs, $"image/jpeg");
                await file.UploadAsync();
                fs.Dispose();
            }
            catch (Exception) { }
            Console.WriteLine("Uploaded!");
        }


        private async Task<YouTubeService> GetService(string SecretsPath, string user)
        {
            UserCredential credential;
            using (var stream = new FileStream(SecretsPath, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync( GoogleClientSecrets.Load(stream).Secrets, new[] { YouTubeService.Scope.YoutubeUpload }, user, CancellationToken.None);
                stream.Dispose();
            }
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            });
            return youtubeService;
        }

    }
}
