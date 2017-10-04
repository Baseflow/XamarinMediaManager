using Android.App;
using Android.Content;
using Android.Content.Res;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;
using Plugin.MediaManager.Audio;

namespace Plugin.MediaManager
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : MediaManagerBase
    {
        public MediaManagerImplementation()
        {
        }

        private IAudioPlayer _audioPlayer;
        public override IAudioPlayer AudioPlayer
        {
            get { return _audioPlayer ?? (_audioPlayer = new AudioPlayerImplementation(this)); }
            set { _audioPlayer = value; }
        }

        private IVideoPlayer _videoPlayer;
        public override IVideoPlayer VideoPlayer
        {
            get { return _videoPlayer ?? (_videoPlayer = new VideoPlayerImplementation(this)); }
            set { _videoPlayer = value; }
        }

        private INotificationManager _notificationManager;
        public override INotificationManager NotificationManager
        {
            get { return _notificationManager ?? (_notificationManager = new NotificationManagerImplementation(this)); }
            set { _notificationManager = value; }
        }

        private IMediaExtractor _mediaExtractor;
        public override IMediaExtractor MediaExtractor
        {
            get { return _mediaExtractor ?? (_mediaExtractor = new MediaExtractorImplementation(this)); }
            set { _mediaExtractor = value; }
        }

        private IVolumeManager _volumeManager;
        public override IVolumeManager VolumeManager
        {
            get { return _volumeManager ?? (_volumeManager = new VolumeManagerImplementation(this)); }
            set { _volumeManager = value; }
        }

        public Context Context { get; } = Application.Context;
    }
}