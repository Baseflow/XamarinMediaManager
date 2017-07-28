using Windows.Media;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;
using Plugin.MediaManager.SystemWrappers;

namespace Plugin.MediaManager
{
    /// <summary>
    ///     Implementation for Feature
    /// </summary>
    public class MediaManagerImplementation : MediaManagerBase
    {
        private IAudioPlayer _audioPlayer;
        private IVideoPlayer _videoPlayer;
        private readonly RemoteControlNotificationHandler _remoteControlNotificationHandler;

        public MediaManagerImplementation()
        {
            var systemMediaTransportControlsWrapper = new SystemMediaTransportControlsWrapper(SystemMediaTransportControls.GetForCurrentView());
            _remoteControlNotificationHandler = new RemoteControlNotificationHandler(systemMediaTransportControlsWrapper, PlaybackController);
            _remoteControlNotificationHandler.SubscribeToNotifications();
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

        public override IVolumeManager VolumeManager { get; set; } = new VolumeManagerImplementation();

        public override void Dispose()
        {
            base.Dispose();
            _remoteControlNotificationHandler.UnsubscribeFromNotifications();
        }
    }
}