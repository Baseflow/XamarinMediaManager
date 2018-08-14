using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Service.Media;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using MediaManager.Audio;
using static MediaManager.Platforms.Android.Temp.Samples;

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

            _mediaSession.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons |
                                   MediaSessionCompat.FlagHandlesTransportControls);

            _mediaSession.Active = true;
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
            return new BrowserRoot(nameof(ApplicationContext.ApplicationInfo.Name), null);
        }

        public override void OnLoadChildren(string parentId, Result result)
        {
            //var test = (Result<List<MediaBrowserCompat.MediaItem>>)result;

            Java.Util.ArrayList list = (Java.Util.ArrayList)FromArray((from aa in SAMPLES
                                                                       select new MediaBrowserCompat.MediaItem(GetMediaDescription(Application.Context, aa), MediaBrowserCompat.MediaItem.FlagPlayable)).ToArray());
            result.SendResult(list);
        }

        public void OnPlaybackStart()
        {
            _delayedStopHandler.RemoveCallbacksAndMessages(null);

            // The service needs to continue running even after the bound client (usually a
            // MediaController) disconnects, otherwise the music playback will stop.
            // Calling startService(Intent) will keep the service running until it is explicitly killed.
            StartService(new Intent(ApplicationContext, typeof(MediaBrowserService)));
        }

        public void OnPlaybackStop()
        {
            // Reset the delayed stop handler, so after STOP_DELAY it will be executed again,
            // potentially stopping the service.
            _delayedStopHandler.RemoveCallbacksAndMessages(null);
            _delayedStopHandler.SendEmptyMessageDelayed(0, STOP_DELAY);
            StopForeground(true);
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
