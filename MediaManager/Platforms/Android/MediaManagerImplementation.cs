using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Platforms.Android;
using MediaManager.Playback;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager
{
    public class MediaManagerImplementation : MediaManagerBase
    {
        public MediaControllerCompat mediaController;
        private MediaBrowserCompat mediaBrowser;
        private ConnectionCallback mConnectionCallback;
        private MediaControllerCallback mMediaControllerCallback;

        public MediaManagerImplementation()
        {
            mMediaControllerCallback = new MediaControllerCallback();

            // Connect a media browser just to get the media session token. There are other ways
            // this can be done, for example by sharing the session token directly.
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            mConnectionCallback = new ConnectionCallback(() =>
            {
                mediaController = new MediaControllerCompat(Application.Context, mediaBrowser.SessionToken);
                mediaController.RegisterCallback(mMediaControllerCallback);
                tcs.SetResult(true);
            });
            mediaBrowser = new MediaBrowserCompat(Application.Context, new ComponentName(Application.Context, nameof(AudioPlayerService)), mConnectionCallback, null);
            mediaBrowser.Connect();
            tcs.Task.Wait();
        }

        private IAudioPlayer _audioPlayer;
        public override IAudioPlayer AudioPlayer
        {
            get
            {
                if (_audioPlayer == null)
                    _audioPlayer = new AudioPlayer();
                return _audioPlayer;
            }
            set
            {
                _audioPlayer = value;
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
                    _volumeManager = new VolumeManager();
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

        private class MediaControllerCallback : MediaControllerCompat.Callback
        {
            public override void OnPlaybackStateChanged(PlaybackStateCompat state)
            {
                base.OnPlaybackStateChanged(state);
            }

            public override void OnMetadataChanged(MediaMetadataCompat metadata)
            {
                base.OnMetadataChanged(metadata);
            }

            public override void OnSessionEvent(string @event, Bundle extras)
            {
                base.OnSessionEvent(@event, extras);
            }
        }

        private class ConnectionCallback : MediaBrowserCompat.ConnectionCallback
        {
            Action OnConnect;

            public ConnectionCallback(Action onConnect)
            {
                OnConnect = onConnect;
            }

            public ConnectionCallback(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
            {

            }

            public override void OnConnected()
            {
                try
                {
                    OnConnect?.Invoke();
                }
                catch (RemoteException e)
                {

                }
            }
        }
    }
}
