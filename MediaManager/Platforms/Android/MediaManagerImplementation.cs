using MediaManager.Media;
using MediaManager.Platforms.Android;
using MediaManager.Playback;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager
{
    public class MediaManagerImplementation : MediaManagerBase
    {
        public MediaManagerImplementation()
        {
        }

        private MediaBrowserManager _mediaBrowserManager;
        public virtual MediaBrowserManager MediaBrowserManager {
            get
            {
                if (_mediaBrowserManager == null)
                    _mediaBrowserManager = new MediaBrowserManager();
                return _mediaBrowserManager;
            }
        }

        private IVideoPlayer _videoPlayer;
        public override IVideoPlayer VideoPlayer
        {
            get
            {
                if (_videoPlayer == null)
                    _videoPlayer = new VideoPlayer();
                return _videoPlayer;
            }
            set
            {
                _videoPlayer = value;
            }
        }

        private INotificationManager _notificationManager;
        public override INotificationManager NotificationManager
        {
            get
            {
                if (_notificationManager == null)
                    _notificationManager = new NotificationManager();
                return _notificationManager;
            }
            set
            {
                _notificationManager = value;
            }
        }

        private IVolumeManager _volumeManager;
        public override IVolumeManager VolumeManager
        {
            get
            {
                if (_volumeManager == null)
                    _volumeManager = new VolumeManager(this);
                return _volumeManager;
            }
            set
            {
                _volumeManager = value;
            }
        }

        private IMediaExtractor _mediaExtractor;
        public override IMediaExtractor MediaExtractor
        {
            get
            {
                if (_mediaExtractor == null)
                    _mediaExtractor = new MediaExtractor();
                return _mediaExtractor;
            }
            set
            {
                _mediaExtractor = value;
            }
        }

        private IPlaybackManager _playbackManager;
        public override IPlaybackManager PlaybackManager
        {
            get
            {
                if (_playbackManager == null)
                    _playbackManager = new PlaybackManager(this);
                return _playbackManager;
            }
            set
            {
                _playbackManager = value;
            }
        }
    }
}
