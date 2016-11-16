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
        private IVolumeManager _volumeManager;
        private IVideoPlayer _videoPlayer;

        public MediaManagerImplementation()
        {
            _volumeManager = new VolumeManagerImplementation();
            _audioPlayer = new AudioPlayerImplementation(_volumeManager);
            _videoPlayer = new VideoPlayerImplementation(_volumeManager);
        }

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

        public override IVolumeManager VolumeManager
        {
            get { return _volumeManager ?? (_volumeManager = new VolumeManagerImplementation()); }
            set { _volumeManager = value; }
        }
    }
}