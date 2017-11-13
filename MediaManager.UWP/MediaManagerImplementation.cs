using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;
using Plugin.MediaManager.Interfaces;

namespace Plugin.MediaManager
{
    /// <summary>
    ///     Implementation for Feature
    /// </summary>
    public class MediaManagerImplementation : MediaManagerBase, IPlaybackControllerProvider
    {
        private IAudioPlayer _audioPlayer;
        private IVideoPlayer _videoPlayer;

        private IMediaNotificationManager _mediaNotificationManager;

        private IMediaPlyerPlaybackController _mediaPlyerPlaybackController;
        private IMediaPlyerPlaybackController MediaPlyerPlaybackController => _mediaPlyerPlaybackController ?? (_mediaPlyerPlaybackController = new MediaPlayerPlaybackController(this));

        public override IAudioPlayer AudioPlayer
        {
            get => _audioPlayer ?? (_audioPlayer = new AudioPlayerImplementation(MediaQueue, MediaPlyerPlaybackController, VolumeManager));
            set => _audioPlayer = value;
        }

        public override IVideoPlayer VideoPlayer
        {
            get => _videoPlayer ?? (_videoPlayer = new VideoPlayerImplementation(MediaQueue, MediaPlyerPlaybackController, VolumeManager));
            set => _videoPlayer = value;
        }

        public override IMediaNotificationManager MediaNotificationManager
        {
            get => _mediaNotificationManager ?? (_mediaNotificationManager = new MediaNotificationManagerImplementation(MediaPlyerPlaybackController));
            set => _mediaNotificationManager = value;
        }

        public override IMediaExtractor MediaExtractor { get; set; } = new MediaExtractorImplementation();

        public override IVolumeManager VolumeManager { get; set; } = new VolumeManagerImplementation();

        #region IDisposable 
        bool disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                (AudioPlayer as BasePlayerImplementation)?.Dispose();
                (VideoPlayer as BasePlayerImplementation)?.Dispose();
            }

            base.Dispose(disposing);

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
        #endregion        
    }
}