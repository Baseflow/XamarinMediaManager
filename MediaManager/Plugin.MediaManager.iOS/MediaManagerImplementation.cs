using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    /// <summary>
    ///     Implementation for MediaManager
    /// </summary>
    public class MediaManagerImplementation : MediaManagerBase
    {
        private IAudioPlayer _audioPlayer;
        private IVideoPlayer _videoPlayer;

        public override IAudioPlayer AudioPlayer
        {
            get { return _audioPlayer ?? (_audioPlayer = new AudioPlayerImplementation(VolumeManager)); }
            set { _audioPlayer = value; }
        }

        public override IVideoPlayer VideoPlayer
        {
            get { return _videoPlayer ?? (_videoPlayer = new VideoPlayerImplementation(VolumeManager)); }
            set { _videoPlayer = value; }
        }

        public override IMediaNotificationManager MediaNotificationManager { get; set; } = new MediaNotificationManagerImplementation();
        public override IMediaExtractor MediaExtractor { get; set; } = new MediaExtractorImplementation();

        public override IVolumeManager VolumeManager { get; set; } = new VolumeManagerImplementation();
    }
}