using System;
using System.Collections.Generic;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager
{
    public interface IMediaManager : IPlaybackManager
    {
        IAudioPlayer AudioPlayer { get; set; }

        IVideoPlayer VideoPlayer { get; set; }

        //INotificationManager NotificationManager { get; set; }

        IMediaExtractor MediaExtractor { get; set; }

        IVolumeManager VolumeManager { get; set; }

        IMediaQueue MediaQueue { get; set; }

        //IPlaybackManager PlaybackManager { get; set; }
    }
}
