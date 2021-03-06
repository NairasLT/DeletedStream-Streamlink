#### About
##### Supports
###### Trovo
###### YouTube

#### Features
###### Uploads Downloaded Livestream to YouTube.
###### Sets the original Title and Description of the Uploaded Livestream.

#### Set-up
###### Run the program.
###### Move your YouTube Api OAuth Json File into Generated Secrets Folder.
###### Configure the Config Json Object in the Config Folder.
###### If no errors appeared the program was configured correctly.
###### Platform 0 = YouTube, Platform 1 = Trovo

#### Configuration

##### Channel Group Async Property: 
###### if false if a streamer has same livestream on different platforms, when the streamer goes live the program will only download one livestream, if true the program will download both livestreams from the different platforms. In short if false starts 1 thread for the channel group if a livestream is found the thread will be blocked. if true starts a thread for each platform.

##### ChannelGroup AsyncMinuteDelay Property:
###### Delay for checking if the streamer is live. Setting this number low may cause a rate limit.

#### Example Config
```
{
  "ChannelGroups": [
    {
      "Channels": [
        {
          "ChannelId": "PewDiePie",
          "MinutesTimeOut": 6, // Used in multithreaded mode.
          "Platform": 1 // Platform Trovo
        },
        {
          "ChannelId": "UC-lHJZR3Gqxm24_Vd_AJ5Yw",
          "MinutesTimeOut": 6, // ignored if running on Singlethread uses AsyncMinuteDelay.
          "Platform": 0 // Platform youtube
        }
      ],
      "Async": false, // Singlethreaded
      "AsyncMinuteDelay": 6 // Singlethreaded mode checking delay
    }
  ]
}
```

