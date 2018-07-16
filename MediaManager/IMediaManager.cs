using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Volume;

namespace MediaManager
{
    public interface IMediaManager
    {
        //IAudioPlayer AudioPlayer { get; set; }

        //IVideoPlayer VideoPlayer { get; set; }

        //INotificationManager NotificationManager { get; set; }

        IMediaExtractor MediaExtractor { get; set; }

        IVolumeManager VolumeManager { get; set; }

        IMediaQueue MediaQueue { get; set; }

        IPlaybackManager PlaybackManager { get; set; }
    }
}
