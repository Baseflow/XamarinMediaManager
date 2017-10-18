using Windows.Media;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;
using Plugin.MediaManager.SystemWrappers;

namespace Plugin.MediaManager
{
    /// <summary>
    ///     Implementation for Feature
    /// </summary>
    public class MediaManagerImplementation : MediaManagerBase, IPlaybackControllerProvider
    {
        private IAudioPlayer _audioPlayer;
        private IVideoPlayer _videoPlayer;
        private readonly MediaButtonPlaybackController _mediaButtonPlaybackController;

        public MediaManagerImplementation()
        {
            var systemMediaTransportControlsWrapper = new SystemMediaTransportControlsWrapper(SystemMediaTransportControls.GetForCurrentView());
            _mediaButtonPlaybackController = new MediaButtonPlaybackController(systemMediaTransportControlsWrapper, this);
            _mediaButtonPlaybackController.SubscribeToNotifications();
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
        public override INotificationManager NotificationManager { get; set; } = new MediaNotificationManagerImplementation();

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
                _mediaButtonPlaybackController.UnsubscribeFromNotifications();
            }

            base.Dispose(disposing);

            // Free any unmanaged objects here.
            //
            disposed = true;                  
        }
        #endregion
    }
}