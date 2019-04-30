using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2.UI;
using MediaManager.Platforms.Android.Media;

namespace MediaManager.Platforms.Android.MediaSession
{
    [Service(Exported = true)]
    [IntentFilter(new[] { global::Android.Service.Media.MediaBrowserService.ServiceInterface })]
    public class MediaBrowserService : MediaBrowserServiceCompat
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Android;

        protected PendingIntent SessionActivityPendingIntent { get; private set; }
        protected MediaSessionCompat MediaSession { get; set; }
        protected MediaDescriptionAdapter MediaDescriptionAdapter { get; set; }
        protected PlayerNotificationManager PlayerNotificationManager { get; set; }
        protected MediaControllerCompat MediaController { get; set; }
        protected MediaControllerCallback MediaControllerCallback { get; set; }
        protected NotificationListener NotificationListener { get; set; }

        public readonly string ChannelId = "audio_channel";
        public readonly int ForegroundNotificationId = 1;

        public MediaBrowserService()
        {
        }

        protected MediaBrowserService(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Intent sessionIntent;
            // Build a PendingIntent that can be used to launch the UI.
            if (MediaManager.GetContext() is Activity activity)
                sessionIntent = new Intent(this, activity.GetType());
            else
                sessionIntent = PackageManager.GetLaunchIntentForPackage(PackageName);
            SessionActivityPendingIntent = PendingIntent.GetActivity(this, 0, sessionIntent, 0);

            PrepareMediaSession();
            PrepareMediaPlayer();
            PrepareNotificationManager();
        }

        protected virtual void PrepareMediaSession()
        {
            MediaSession = new MediaSessionCompat(this, nameof(MediaBrowserService));
            MediaSession.SetSessionActivity(SessionActivityPendingIntent);
            MediaSession.Active = true;

            SessionToken = MediaSession.SessionToken;

            MediaSession.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons |
                                   MediaSessionCompat.FlagHandlesTransportControls);
        }

        protected virtual void PrepareMediaPlayer()
        {
            MediaManager.AndroidMediaPlayer.MediaSession = MediaSession;
            MediaManager.MediaPlayer.Initialize();
        }

        protected virtual void PrepareNotificationManager()
        {
            MediaDescriptionAdapter = new MediaDescriptionAdapter(SessionActivityPendingIntent);
            PlayerNotificationManager = Com.Google.Android.Exoplayer2.UI.PlayerNotificationManager.CreateWithNotificationChannel(
                this,
                ChannelId,
                Resource.String.exo_download_notification_channel_name,
                ForegroundNotificationId,
                MediaDescriptionAdapter);

            //Needed for enabling the notification as a mediabrowser.
            NotificationListener = new NotificationListener();
            NotificationListener.OnNotificationStartedImpl = (notificationId, notification) =>
            {
                StartForeground(notificationId, notification);
            };
            NotificationListener.OnNotificationCancelledImpl = (notificationId) =>
            {
                StopForeground(true);
            };

            MediaManager.MediaQueue.CollectionChanged += MediaQueue_CollectionChanged;

            PlayerNotificationManager.SetFastForwardIncrementMs((long)MediaManager.StepSize.TotalMilliseconds);
            PlayerNotificationManager.SetRewindIncrementMs((long)MediaManager.StepSize.TotalMilliseconds);
            PlayerNotificationManager.SetNotificationListener(NotificationListener);
            PlayerNotificationManager.SetMediaSessionToken(SessionToken);
            PlayerNotificationManager.SetPlayer(MediaManager.AndroidMediaPlayer.Player);
        }

        private void MediaQueue_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (MediaManager.MediaQueue.Count > 1)
            {
                PlayerNotificationManager?.SetUseNavigationActions(true);
            }
            else
            {
                PlayerNotificationManager?.SetUseNavigationActions(false);
            }
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
            MediaManager.MediaPlayer.Dispose();
            MediaManager.MediaPlayer = null;
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
