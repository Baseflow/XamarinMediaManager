using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager.Platforms.Mac
{
    public class MediaManagerImplementation : MediaManagerBase
    {
        //public override IAudioPlayer AudioPlayer { get; set; }
        public override IVideoPlayer VideoPlayer { get; set; }
        public override INotificationManager NotificationManager { get; set; }
        public override IMediaExtractor MediaExtractor { get; set; }
        public override IVolumeManager VolumeManager { get; set; }
        public override IPlaybackManager PlaybackManager { get; set; }
    }
}
