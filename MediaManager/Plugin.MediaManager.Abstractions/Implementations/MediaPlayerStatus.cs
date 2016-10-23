using System;
namespace Plugin.MediaManager.Abstractions.Implementations
{
    public enum MediaPlayerStatus
    {
        Stopped,
        Paused,
        Playing,
        Loading,
        Buffering,
        Failed
    }
}

