using System.Collections.Generic;

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

}