using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ChannelGroup
{
    public IList<Channel> Channels = new List<Channel>();
    public bool Async = false;
    public int AsyncMinuteDelay = 6;

    public ChannelGroup(IList<Channel> channels, bool async, int asyncMinuteDelay)
    {
        Channels = channels;
        Async = async;
        AsyncMinuteDelay = asyncMinuteDelay;
    }


    public static void AsynchronousInfiniteStart(ChannelGroup group)
    {
        foreach (var user in group.Channels)
        {
            if (user.ChannelId == FilePaths.ConfigExampleText) { CError.ErrorExampleObjectFound(); return; }

            CMessage.InctanceStarted(user, true);


            switch (user.Platform)
            {
                case Platform.Trovo:
                    var trovo = new TrovoPlugin(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                    _ = trovo.RunInfinite(); // Discard await basically creates a new thread.
                    break;

                case Platform.YouTube:
                    var Runtime = new YouTubePlugin(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                    _ = Runtime.RunInfinite();
                    break;
            }
        }
    }

    public async static Task SynchronousInfinite(ChannelGroup group)
    {
        while (true)
        {
            foreach (var user in group.Channels)
            {
                if (user.ChannelId == FilePaths.ConfigExampleText) { CError.ErrorExampleObjectFound(); return; }

                CMessage.InctanceStarted(user, false);

                switch (user.Platform)
                {

                    case Platform.Trovo:
                        var trovo = new TrovoPlugin(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                        await trovo.Run();
                        break;

                    case Platform.YouTube:
                        var Runtime = new YouTubePlugin(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                        await Runtime.Run();
                        break;
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(group.AsyncMinuteDelay));
        }
    }

}