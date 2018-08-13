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
    [IntentFilter(new[] { global::Android.Service.Media.MediaBrowserService.ServiceInterface })]
    public class MediaBrowserService : MediaBrowserServiceCompat
    {
        private IAudioPlayer _audioPlayer;
        public virtual IAudioPlayer AudioPlayer
        {
            get
            {
                if (_audioPlayer == null)
                    _audioPlayer = new AudioPlayer(_mediaSession);
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

        public MediaBrowserService()
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            _mediaSession = new MediaSessionCompat(this, nameof(MediaBrowserService));
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

            mediaCallback.OnPlayImpl = () =>
            {
                if (!_mediaSession.Active)
                    _mediaSession.Active = true;

                throw new NotImplementedException("Testing Play implementation!");

                AudioPlayer.Play();
            };

            mediaCallback.OnSkipToQueueItemImpl = (long id) =>
            {
                throw new NotImplementedException();
                //AudioPlayer.SkipToQueueItem(id);
            };

            mediaCallback.OnSeekToImpl = (long pos) =>
            {
                throw new NotImplementedException("Testing Seek implementation!");

                AudioPlayer.Seek(TimeSpan.FromMilliseconds(pos));
            };

            mediaCallback.OnPlayFromMediaIdImpl = (string mediaId, Bundle bundle) =>
            {
                throw new NotImplementedException();
                //AudioPlayer.PlayFromMediaId(mediaId, bundle);
            };

            mediaCallback.OnPauseImpl = () =>
            {
                AudioPlayer.Pause();
            };

            mediaCallback.OnStopImpl = () =>
            {
                AudioPlayer.Stop();
            };

            mediaCallback.OnSkipToNextImpl = () =>
            {
                throw new NotImplementedException();
                //AudioPlayer.SkipToNext();
            };

            mediaCallback.OnSkipToPreviousImpl = () =>
            {
                throw new NotImplementedException();
                //AudioPlayer.SkipToPrevious();
            };

            mediaCallback.OnCustomActionImpl = (string action, Bundle bundle) =>
            {
                throw new NotImplementedException();
                //AudioPlayer.CustomAction(action, bundle);
            };

            mediaCallback.OnPlayFromSearchImpl = (string query, Bundle bundle) =>
            {
                throw new NotImplementedException();
                //AudioPlayer.PlayFromSearch(action, bundle);
            };

            mediaCallback.OnPlayFromUriImpl = (uri, bundle) =>
            {
                if (!_mediaSession.Active)
                    _mediaSession.Active = true;

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

        public void OnPlaybackStart()
        {
            if (!_mediaSession.Active)
            {
                _mediaSession.Active = true;
            }

            _delayedStopHandler.RemoveCallbacksAndMessages(null);

            // The service needs to continue running even after the bound client (usually a
            // MediaController) disconnects, otherwise the music playback will stop.
            // Calling startService(Intent) will keep the service running until it is explicitly killed.
            StartService(new Intent(ApplicationContext, typeof(MediaBrowserService)));
        }

        public void OnNotificationRequired()
        {
            //_mediaNotificationManager.StartNotification();
        }

        public void OnPlaybackStop()
        {
            // Reset the delayed stop handler, so after STOP_DELAY it will be executed again,
            // potentially stopping the service.
            _delayedStopHandler.RemoveCallbacksAndMessages(null);
            _delayedStopHandler.SendEmptyMessageDelayed(0, STOP_DELAY);
            StopForeground(true);
        }

        public void OnPlaybackStateUpdated(PlaybackStateCompat newState)
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
                OnPlayImpl?.Invoke();
            }

            public override void OnSkipToQueueItem(long id)
            {
                OnSkipToQueueItemImpl?.Invoke(id);
            }

            public override void OnSeekTo(long pos)
            {
                OnSeekToImpl?.Invoke(pos);
            }

            public override void OnPlayFromMediaId(string mediaId, Bundle extras)
            {
                OnPlayFromMediaIdImpl?.Invoke(mediaId, extras);
            }

            public override void OnPlayFromUri(global::Android.Net.Uri uri, Bundle extras)
            {
                //throw new NotImplementedException("Not implemented...");
                OnPlayFromUriImpl?.Invoke(uri, extras);
            }

            public override void OnPause()
            {
                OnPauseImpl?.Invoke();
            }

            public override void OnStop()
            {
                OnStopImpl?.Invoke();
            }

            public override void OnSkipToNext()
            {
                OnSkipToNextImpl?.Invoke();
            }

            public override void OnSkipToPrevious()
            {
                OnSkipToPreviousImpl?.Invoke();
            }

            public override void OnCustomAction(string action, Bundle extras)
            {
                OnCustomActionImpl?.Invoke(action, extras);
            }

            public override void OnPlayFromSearch(string query, Bundle extras)
            {
                OnPlayFromSearchImpl?.Invoke(query, extras);
            }
        }

        /**
        * A simple handler that stops the service if playback is not active (playing)
        */
        public class DelayedStopHandler : Handler
        {
            private WeakReference<MediaBrowserService> _weakReference;

            public DelayedStopHandler(MediaBrowserService service)
            {
                _weakReference = new WeakReference<MediaBrowserService>(service);
            }

            public override void HandleMessage(Message msg)
            {
                MediaBrowserService service;
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
