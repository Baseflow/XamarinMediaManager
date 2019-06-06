using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
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

            PrepareMediaSession();

            if(MediaManager.NotificationManager.Enabled)
                PrepareNotificationManager();
        }

        protected virtual void PrepareMediaSession()
        {
            MediaSession = new MediaSessionCompat(this, nameof(MediaBrowserService));
            MediaSession.SetSessionActivity(MediaManager.SessionActivityPendingIntent);
            MediaSession.Active = true;

            SessionToken = MediaSession.SessionToken;

            MediaSession.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons |
                                   MediaSessionCompat.FlagHandlesTransportControls);

            MediaManager.AndroidMediaPlayer.MediaSession = MediaSession;
        }

        protected virtual void PrepareNotificationManager()
        {
            MediaDescriptionAdapter = new MediaDescriptionAdapter();
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
                ContextCompat.StartForegroundService(ApplicationContext, new Intent(ApplicationContext, Java.Lang.Class.FromType(typeof(MediaBrowserService))));
                StartForeground(notificationId, notification);
            };
            NotificationListener.OnNotificationCancelledImpl = (notificationId) =>
            {
                //TODO: in new exoplayer use StopForeground(false) or use dismissedByUser
                StopForeground(true);
                StopSelf();
            };

            PlayerNotificationManager.SetFastForwardIncrementMs((long)MediaManager.StepSize.TotalMilliseconds);
            PlayerNotificationManager.SetRewindIncrementMs((long)MediaManager.StepSize.TotalMilliseconds);
            PlayerNotificationManager.SetNotificationListener(NotificationListener);
            PlayerNotificationManager.SetMediaSessionToken(SessionToken);
            PlayerNotificationManager.SetOngoing(true);
            PlayerNotificationManager.SetPlayer(MediaManager.AndroidMediaPlayer.Player);

            PlayerNotificationManager.SetUsePlayPauseActions(MediaManager.NotificationManager.ShowPlayPauseControls);
            PlayerNotificationManager.SetUseNavigationActions(MediaManager.NotificationManager.ShowNavigationControls);

            MediaManager.MediaQueue.QueueChanged += MediaQueue_QueueChanged;
        }

        private void MediaQueue_QueueChanged(object sender, Queue.QueueChangedEventArgs e)
        {
            //TODO: Call PlayerNotificationManager.Invalidate(); on exoplayer 2.9.6 when metadata is updated

            if (PlayerNotificationManager != null)
            {
                if (MediaManager.NotificationManager.ShowNavigationControls && MediaManager.MediaQueue.Count > 1)
                {
                    PlayerNotificationManager?.SetUseNavigationActions(true);
                }
                else
                {
                    PlayerNotificationManager?.SetUseNavigationActions(false);
                }
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
            //MediaManager.MediaQueue.QueueChanged -= MediaQueue_QueueChanged;
            //PlayerNotificationManager.SetPlayer(null);
            //PlayerNotificationManager.Dispose();
            //MediaManager.MediaPlayer.Dispose();
            //MediaManager.MediaPlayer = null;
            MediaSession.Active = false;
            MediaSession.Release();
            //MediaSession = null;
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
