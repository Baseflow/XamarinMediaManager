using System;
using Windows.Media.Playback;

namespace Plugin.MediaManager.Interfaces
{
    public interface IMediaPlyerPlaybackController : IDisposable
    {
        MediaPlayer Player { get; }
    }
}
