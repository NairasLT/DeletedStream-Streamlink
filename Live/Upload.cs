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
using System.Linq;
using System.Text.RegularExpressions;

class Upload
    {
    public string file;

    public async Task Run(string Filename)
    {

        Console.WriteLine("FILE NAME FOR UPLOAD: " + Filename);

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
             video.Snippet.Description = "Ikelta Automatiskai.\nhttps://github.com/NairasLT/Checking-Downloading-Uploading-Livestream_using_streamlink";
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
         catch (Exception ex)
         {
             Console.WriteLine("Execption #1 wow congrats: " + ex.Message);

             if(ex.Message.Contains("Could not find file"))
             {
                 writeDeleteLineUploaded(file);
             }

         }

    }

    public void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                Console.WriteLine(string.Format("{0} bytes sent.", progress.BytesSent));
                    break;

                case UploadStatus.Failed:
                Console.WriteLine(string.Format(" -------->  An error prevented the upload from completing.\n{0}", progress.Exception));

                if (progress.Exception.ToString().Contains("Response status code does not indicate success: 403 (Forbidden)"))
                {
                    Console.WriteLine("\n\nQUOTA LIMITED! TRYING TO UPLOAD IN 3 HOURS\n\nCHECKING WILL BE CONTINUED THIS WAITING IS ON ANOTHER THREAD.");
                    Thread.Sleep(10800000);
                }
                break;
            }
        }

        public void videosInsertRequest_ResponseReceived(Video video)
        {
        Console.WriteLine(string.Format("Video id '{0}' was successfully uploaded.", video.Id));
        writeDeleteLineUploaded(file);
        }


    public void writenotDone(string FileNotUploaded)
    {
        string pth = new Other().NotDomePath; // wow you can do instance of string. not of the intire class.

        using (StreamWriter sw = File.AppendText(pth))
        {
            sw.WriteLine(FileNotUploaded);
            sw.Close();
        }
    }

    public void writeDeleteLineUploaded(string FileNameToDelete)
    {
        string path = new Other().NotDomePath;
        string path1 = new Other().MainPath;
        string item = FileNameToDelete;
        var lines = File.ReadAllLines(path).Where(line => line.Trim() != item).ToArray();
        File.WriteAllLines(path, lines);
    }

    public string getFirstLineOfFile(string Filepath)
    {
        string line1 = File.ReadLines(Filepath).First();
        return line1;
    }

    public void UploadVideoOnNewThread(string FileNameToUplaod) // NOT SURE IF WORKS!!!!!!! <-------
    {
        try
        {

            Thread thr = new Thread(upload);
            thr.Start();
            Console.WriteLine("Created Thread for:" + FileNameToUplaod);
            async void upload()
            {
                await Run(FileNameToUplaod);
                Console.WriteLine("Uploading for: " + FileNameToUplaod);
            }
        }
        catch (Exception ex2)
        {
            Console.WriteLine("Error, void UploadVideoOnNewThread: " + ex2.Message);
        }

    }

}
