using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2.UI;
using MediaManager.Audio;
using MediaManager.Platforms.Android.Audio;
using MediaManager.Platforms.Android.Media;
using MediaManager.Playback;

namespace MediaManager.Platforms.Android.MediaSession
{
    [Service(Exported = true)]
    [IntentFilter(new[] { global::Android.Service.Media.MediaBrowserService.ServiceInterface })]
    public class MediaBrowserService : MediaBrowserServiceCompat
    {
        protected IMediaManager MediaManager = CrossMediaManager.Current;

        private IAudioPlayer _audioPlayer;
        protected IAudioPlayer AudioPlayer
        {
            get
            {
                if (_audioPlayer == null)
                    _audioPlayer = MediaManager.AudioPlayer;

                return _audioPlayer;
            }
            set
            {
                MediaManager.AudioPlayer = value;
                _audioPlayer = value;
            }
        }

        protected AudioPlayer NativePlayer => AudioPlayer as AudioPlayer;

        protected MediaSessionCompat MediaSession { get; set; }
        protected MediaDescriptionAdapter MediaDescriptionAdapter { get; set; }
        protected PlayerNotificationManager PlayerNotificationManager { get; set; }
        protected MediaControllerCompat MediaController { get; set; }
        protected MediaControllerCallback MediaControllerCallback { get; set; }
        protected BecomingNoisyReceiver BecomingNoisyReceiver { get; set; }
        protected NotificationListener NotificationListener { get; set; }

        public readonly string ChannelId = "audio_channel";
        public readonly int ForegroundNotificationId = 1;

        public MediaBrowserService()
        {
        }

        public MediaBrowserService(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            // Build a PendingIntent that can be used to launch the UI.
            var sessionIntent = PackageManager.GetLaunchIntentForPackage(PackageName);
            var sessionActivityPendingIntent = PendingIntent.GetActivity(this, 0, sessionIntent, 0);

            MediaSession = new MediaSessionCompat(this, nameof(MediaBrowserService));
            MediaSession.SetSessionActivity(sessionActivityPendingIntent);
            MediaSession.Active = true;

            SessionToken = MediaSession.SessionToken;

            MediaSession.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons |
                                   MediaSessionCompat.FlagHandlesTransportControls);

            NativePlayer.MediaSession = MediaSession;
            AudioPlayer.Initialize();

            MediaDescriptionAdapter = new MediaDescriptionAdapter(sessionActivityPendingIntent);
            PlayerNotificationManager = Com.Google.Android.Exoplayer2.UI.PlayerNotificationManager.CreateWithNotificationChannel(
                this,
                ChannelId,
                Resource.String.exo_download_notification_channel_name,
                ForegroundNotificationId,
                new MediaDescriptionAdapter(sessionActivityPendingIntent));

            //needed for enabling the notification as a mediabrowser.
            PlayerNotificationManager.SetMediaSessionToken(SessionToken);
            NotificationListener = new NotificationListener();
            PlayerNotificationManager.SetNotificationListener(NotificationListener);
            PlayerNotificationManager.SetPlayer(NativePlayer.Player);

            //Everything after this probably needs to be removed. 
            BecomingNoisyReceiver = new BecomingNoisyReceiver(MediaManager.GetContext(), NativePlayer.AudioFocusManager);
            MediaController = new MediaControllerCompat(this, MediaSession);

            MediaControllerCallback = new MediaControllerCallback()
            {
                OnPlaybackStateChangedImpl = (state) =>
                {
                    if (state.State == PlaybackStateCompat.StatePlaying || state.State == PlaybackStateCompat.StateBuffering)
                        BecomingNoisyReceiver.Register();
                    else
                        BecomingNoisyReceiver.Unregister();
                }
            };
            MediaController.RegisterCallback(MediaControllerCallback);
        }

        public override StartCommandResult OnStartCommand(Intent startIntent, StartCommandFlags flags, int startId)
        {
            if (startIntent != null)
            {
                MediaButtonReceiver.HandleIntent(MediaSession, startIntent);
            }
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            // Service is being killed, so make sure we release our resources
            PlayerNotificationManager.SetPlayer(null);
            NativePlayer.Dispose();
            AudioPlayer = null;
            MediaSession.Release();
            StopForeground(true);
        }

        public override BrowserRoot OnGetRoot(string clientPackageName, int clientUid, Bundle rootHints)
        {
            return new BrowserRoot(nameof(ApplicationContext.ApplicationInfo.Name), null);
        }

        public override void OnLoadChildren(string parentId, Result result)
        {
            var mediaItems = new JavaList<MediaBrowserCompat.MediaItem>();

            foreach (var item in MediaManager.MediaQueue)
                mediaItems.Add(item.ToMediaBrowserMediaItem());

            result.SendResult(mediaItems);
        }
    }
}
