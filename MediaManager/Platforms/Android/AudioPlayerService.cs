using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Service.Media;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using MediaManager.Audio;

namespace MediaManager.Platforms.Android
{
    [Service(Exported = true)]
    [IntentFilter(new[] { MediaBrowserService.ServiceInterface })]
    public class AudioPlayerService : MediaBrowserServiceCompat
    {
        private IAudioPlayer _audioPlayer;
        public virtual IAudioPlayer AudioPlayer
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

        private MediaSessionCompat _mediaSession;
        //private MediaRouter _mediaRouter;

        private DelayedStopHandler _delayedStopHandler;
        private int STOP_DELAY = 30000;

        public AudioPlayerService()
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            _mediaSession = new MediaSessionCompat(this, nameof(AudioPlayerService));
            SessionToken = _mediaSession.SessionToken;

            var mediaCallback = new MediaSessionCallback();
            _mediaSession.SetCallback(mediaCallback);

            _mediaSession.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons |
                                   MediaSessionCompat.FlagHandlesTransportControls);

            //Context context = ApplicationContext;
            //var intent = new Intent(context, typeof(MusicPlayerActivity));
            //var pi = PendingIntent.GetActivity(context, 99 /*request code*/,
            //             intent, PendingIntentFlags.UpdateCurrent);
            //_mediaSession.SetSessionActivity(pi);

            //_mediaRouter = MediaRouter.GetInstance(ApplicationContext);

            mediaCallback.OnPlayFromUriImpl = (uri, bundle) =>
            {
                AudioPlayer.Play(uri.ToString());
            };
        }

        public override StartCommandResult OnStartCommand(Intent startIntent, StartCommandFlags flags, int startId)
        {
            if (startIntent != null)
            {
                MediaButtonReceiver.HandleIntent(_mediaSession, startIntent);
            }

            // Reset the delay handler to enqueue a message to stop the service if
            // nothing is playing.
            _delayedStopHandler.RemoveCallbacksAndMessages(null);
            _delayedStopHandler.SendEmptyMessageDelayed(0, STOP_DELAY);
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            //unregisterCarConnectionReceiver();
            // Service is being killed, so make sure we release our resources
            //mPlaybackManager.handleStopRequest(null);
            //_mediaNotificationManager.StopNotifications();

            //Maybe use for communication
            //_mediaSession.Controller.SendCommand();


            /*
            if (mCastSessionManager != null)
            {
                mCastSessionManager.removeSessionManagerListener(mCastSessionManagerListener,
                        CastSession.class);
            }
            */
            _delayedStopHandler.RemoveCallbacksAndMessages(null);
            _mediaSession.Release();
        }

        public override BrowserRoot OnGetRoot(string clientPackageName, int clientUid, Bundle rootHints)
        {
            return new BrowserRoot(nameof(ApplicationContext.ApplicationInfo.Name), // Name visible in Android Auto
                null);
        }

        public override void OnLoadChildren(string parentId, Result result)
        {
            //var test = (Result<List<MediaBrowserCompat.MediaItem>>)result;
            result.SendResult(null);
        }

        public void onPlaybackStart()
        {
            if (!_mediaSession.Active)
            {
                _mediaSession.Active = true;
            }

            _delayedStopHandler.RemoveCallbacksAndMessages(null);

            // The service needs to continue running even after the bound client (usually a
            // MediaController) disconnects, otherwise the music playback will stop.
            // Calling startService(Intent) will keep the service running until it is explicitly killed.
            StartService(new Intent(ApplicationContext, typeof(AudioPlayerService)));
        }

        public void onNotificationRequired()
        {
            //_mediaNotificationManager.StartNotification();
        }

        public void onPlaybackStop()
        {
            // Reset the delayed stop handler, so after STOP_DELAY it will be executed again,
            // potentially stopping the service.
            _delayedStopHandler.RemoveCallbacksAndMessages(null);
            _delayedStopHandler.SendEmptyMessageDelayed(0, STOP_DELAY);
            StopForeground(true);
        }

        public void onPlaybackStateUpdated(PlaybackStateCompat newState)
        {
            _mediaSession.SetPlaybackState(newState);
        }

        class MediaSessionCallback : MediaSessionCompat.Callback
        {
            public Action OnPlayImpl { get; set; }

            public Action<long> OnSkipToQueueItemImpl { get; set; }

            public Action<long> OnSeekToImpl { get; set; }

            public Action<string, Bundle> OnPlayFromMediaIdImpl { get; set; }

            public Action OnPauseImpl { get; set; }

            public Action OnStopImpl { get; set; }

            public Action OnSkipToNextImpl { get; set; }

            public Action OnSkipToPreviousImpl { get; set; }

            public Action<string, Bundle> OnCustomActionImpl { get; set; }

            public Action<string, Bundle> OnPlayFromSearchImpl { get; set; }

            public Action<global::Android.Net.Uri, Bundle> OnPlayFromUriImpl { get; set; }

            public override void OnPlay()
            {
                OnPlayImpl();
            }

            public override void OnSkipToQueueItem(long id)
            {
                OnSkipToQueueItemImpl(id);
            }

            public override void OnSeekTo(long pos)
            {
                OnSeekToImpl(pos);
            }

            public override void OnPlayFromMediaId(string mediaId, Bundle extras)
            {
                OnPlayFromMediaIdImpl(mediaId, extras);
            }

            public override void OnPlayFromUri(global::Android.Net.Uri uri, Bundle extras)
            {
                OnPlayFromUriImpl(uri, extras);
            }

            public override void OnPause()
            {
                OnPauseImpl();
            }

            public override void OnStop()
            {
                OnStopImpl();
            }

            public override void OnSkipToNext()
            {
                OnSkipToNextImpl();
            }

            public override void OnSkipToPrevious()
            {
                OnSkipToPreviousImpl();
            }

            public override void OnCustomAction(string action, Bundle extras)
            {
                OnCustomActionImpl(action, extras);
            }

            public override void OnPlayFromSearch(string query, Bundle extras)
            {
                OnPlayFromSearchImpl(query, extras);
            }
        }

        /**
        * A simple handler that stops the service if playback is not active (playing)
        */
        public class DelayedStopHandler : Handler
        {
            private WeakReference<AudioPlayerService> _weakReference;

            public DelayedStopHandler(AudioPlayerService service)
            {
                _weakReference = new WeakReference<AudioPlayerService>(service);
            }

            public override void HandleMessage(Message msg)
            {
                AudioPlayerService service;
                if (_weakReference.TryGetTarget(out service))
                {
                    if (service?._mediaSession?.Controller?.PlaybackState?.State != null)
                    {
                        if (service?._mediaSession?.Controller?.PlaybackState.State == PlaybackStateCompat.StatePlaying)
                        {
                            return;
                        }
                        service.StopSelf();
                    }
                }
            }
        }
    }
}
