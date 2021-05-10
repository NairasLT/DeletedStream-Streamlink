using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PluginHandler // this is so messy i could do this alot cleaner with typescript, i cannot with c#
{
    public static void Async(ChannelGroup group)
    {
        foreach (var user in group.Channels)
        {
            if (user.ChannelId == FilePaths.ConfigExampleText) { CError.ErrorExampleObjectFound(); return; }

            CMessage.InctanceStarted(user, true);

            switch (user.Platform)
            {
                case Platform.Trovo:
                    var TrovoGQL = new TrovoPluginGQL(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                    _ = TrovoGQL.RunInfinite();
                    break;
                case Platform.TrovoDeprecated:
                    var OldTrovo = new TrovoPlugin(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                    _ = OldTrovo.RunInfinite(); // Discard await basically creates a new thread.
                    break;

                case Platform.YouTube:
                    var Runtime = new YouTubePlugin(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                    _ = Runtime.RunInfinite();
                    break;
            }
        }
    }

    public async static Task Sync(ChannelGroup group)
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
                        var TrovoGQL = new TrovoPluginGQL(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                        await TrovoGQL.Run();
                        break;
                    case Platform.TrovoDeprecated:
                        var OldTrovo = new TrovoPlugin(user.ChannelId, TimeSpan.FromMinutes(user.MinutesTimeOut));
                        await OldTrovo.Run(); // Discard await basically creates a new thread.
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
    public static void HandlePlugn(ChannelGroup group)
    {
        if (group.Async) PluginHandler.Async(group);
        else _ = PluginHandler.Sync(group);
    }
}
