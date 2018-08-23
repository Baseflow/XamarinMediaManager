using Android.App;
using Android.Content;
using MediaManager.Media;
using MediaManager.Platforms.Android;
using MediaManager.Playback;
using MediaManager.Video;
using MediaManager.Volume;
using MediaManager.Platforms.Android.Audio;
using MediaManager.Platforms.Android.ServiceBinder;
using System.Threading.Tasks;
using MediaManager.Platforms.Android.Utils;

namespace MediaManager
{
    public class MediaManagerImplementation : MediaManagerBase, IMediaBrowserServiceCallback
    {
        MediaBrowserService _service;

        public MediaManagerImplementation()
        {
            MediaBrowserManager.EnsureInitialized();

            #region servicebinding
            mediaPlayerServiceIntent = new Intent(Application.Context, typeof(MediaBrowserService));
            mediaBrowserServiceConnection = new MediaBrowserServiceConnection(this);
            Application.Context.BindService(mediaPlayerServiceIntent, mediaBrowserServiceConnection, Bind.AutoCreate);
            #endregion
        }

        public Context Context { get; set; } = Application.Context;

        private MediaBrowserManager _mediaBrowserManager;
        public virtual MediaBrowserManager MediaBrowserManager
        {
            get
            {
                if (_mediaBrowserManager == null)
                    _mediaBrowserManager = new MediaBrowserManager(this);
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

        public void UpdateCurrentPlaying(int index)
        {
            //((AndroidMediaQueue)_mediaQueue).UpdateQueue(index);
        }

        public void UpdateMedia()
        {
            throw new System.NotImplementedException();
        }

        #region ServiceBinding
        private MediaBrowserServiceBinder binder;
        private Intent mediaPlayerServiceIntent;
        private MediaBrowserServiceConnection mediaBrowserServiceConnection;
        private bool isBound = false;

        public void UnbindMediaPlayerService()
        {
            if (isBound)
            {
                Application.Context.UnbindService(mediaBrowserServiceConnection);
                isBound = false;
            }
        }

        internal void OnServiceConnected(MediaBrowserServiceBinder serviceBinder)
        {
            binder = serviceBinder;
            isBound = true;
            _service = binder.GetMediaPlayerService();

            _service.SetQueue((MediaQueue)MediaQueue);
        }

        internal void OnServiceDisconnected()
        {
            isBound = false;
        }

        private async Task BinderReady()
        {
            int repeat = 10;
            while ((binder == null) && (repeat > 0))
            {
                await Task.Delay(100);
                repeat--;
            }
            if (repeat == 0)
            {
                throw new System.TimeoutException("Could not connect to service");
            }
        }
        #endregion
    }
}
