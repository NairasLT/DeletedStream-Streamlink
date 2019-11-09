using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Live;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

    class Upload
    {
        Other log = new Other();
        string file;
    public async void Run(string Filename)
    {
        try
        {
            file = Filename;
            UserCredential credential;
            using (var stream = new FileStream(@"C:\client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows an application to upload files to the
                    // authenticated user's YouTube channel, but doesn't allow other types of access.
                    new[] { YouTubeService.Scope.YoutubeUpload },
                    "user",
                    CancellationToken.None
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = DateTime.Now.ToString();
            video.Snippet.Description = "AUTO UPLOADED C#";
            video.Snippet.Tags = new string[] { "auto", "upload" };
            video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "unlisted"; // or "private" or "public"
            Console.WriteLine("FILE NAME FOR UPLOAD: " + Filename);
            var filePath = Filename; // Replace with path to actual movie file.

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                await videosInsertRequest.UploadAsync();
            }
        }
        catch (Exception)
        {
            log.WriteLine("Execption #1 wow congrats");
        }
    }

    public void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                log.WriteLine(string.Format("{0} bytes sent.", progress.BytesSent));
                    break;

                case UploadStatus.Failed:
                    log.WriteLine(string.Format(" -------->  An error prevented the upload from completing.\n{0}", progress.Exception));
                break;
            }
        }

        public void videosInsertRequest_ResponseReceived(Video video)
        {
        log.WriteLine(string.Format("Video id '{0}' was successfully uploaded.", video.Id));
    }







}
