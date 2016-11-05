using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;
using NotificationCompat = Android.Support.V7.App.NotificationCompat;

namespace Plugin.MediaManager.MediaSession
{
    internal class MediaNotificationManager: IMediaNotificationManager
    {
       // private MediaSessionManagerImplementation _sessionHandler;
        private Intent _intent;
        private PendingIntent _pendingCancelIntent;
        private PendingIntent _pendingIntent;
        private NotificationCompat.MediaStyle _notificationStyle = new NotificationCompat.MediaStyle();
        private MediaSessionCompat.Token _sessionToken;
        private Context _appliactionContext;
        private NotificationCompat.Builder _builder;

        public MediaNotificationManager(Context appliactionContext, MediaSessionCompat.Token sessionToken)
        {
            _sessionToken = sessionToken;
            _appliactionContext = appliactionContext;
            _intent = new Intent(_appliactionContext, typeof(MediaPlayerService.MediaPlayerService));
        }

        /// <summary>
        /// Starts the notification.
        /// </summary>
        /// <param name="mediaFile">The media file.</param>
        public void StartNotification(IMediaFile mediaFile)
        {
            StartNotification(mediaFile, true);
        }

        /// <summary>
        /// When we start on the foreground we will present a notification to the user
        /// When they press the notification it will take them to the main page so they can control the music
        /// </summary>
        public void StartNotification(IMediaFile mediaFile, bool mediaIsPlaying)
        {
            _intent.SetAction(MediaPlayerService.MediaPlayerService.ActionStop);
            _pendingCancelIntent = PendingIntent.GetService(_appliactionContext, 1, _intent, PendingIntentFlags.CancelCurrent);
            _notificationStyle.SetMediaSession(_sessionToken);
            _notificationStyle.SetShowCancelButton(mediaIsPlaying);
            _notificationStyle.SetCancelButtonIntent(_pendingCancelIntent);
            _notificationStyle.SetShowActionsInCompactView(0, 1, 2);

            _builder = new NotificationCompat.Builder(_appliactionContext)
            {
                MStyle = _notificationStyle
            };
            _builder.SetColor(ContextCompat.GetColor(_appliactionContext, Resource.Color.BackgroundDark));
            _builder.SetSmallIcon(Resource.Drawable.IcMediaPlay);
            _builder.SetContentIntent(_pendingIntent);
            _builder.SetShowWhen(mediaIsPlaying);
            _builder.SetOngoing(mediaIsPlaying);
            _builder.SetVisibility(1);
           
            SetMetadata(mediaFile);
            AddActionButtons(mediaIsPlaying);

            NotificationManagerCompat.From(_appliactionContext).Notify(MediaPlayerService.MediaPlayerService.NotificationId, _builder.Build());
        }


        public void StopNotifications()
        {
            NotificationManagerCompat nm = NotificationManagerCompat.From(_appliactionContext);
            nm.CancelAll();
        }

        public void UpdateNotifications(IMediaFile mediaFile, MediaPlayerStatus status)
        {
            var isPlaying = status == MediaPlayerStatus.Playing || status == MediaPlayerStatus.Buffering;
            var nm = NotificationManagerCompat.From(_appliactionContext);
            if (nm != null && _builder != null)
            {
                SetMetadata(mediaFile);
                AddActionButtons(isPlaying);
                nm.Notify(MediaPlayerService.MediaPlayerService.NotificationId, _builder.Build());
            }
            else
            {
                StartNotification(mediaFile, isPlaying);
            }
        }

        private void SetMetadata(IMediaFile mediaFile)
        {
            _builder.SetContentTitle(mediaFile.Metadata.Title);
            _builder.SetContentText(mediaFile.Metadata.Artist);
            _builder.SetContentInfo(mediaFile.Metadata.Album);
            _builder.SetLargeIcon(mediaFile.Metadata.Cover as Bitmap);
        }

        private Android.Support.V4.App.NotificationCompat.Action GenerateActionCompat(int icon, string title, string intentAction)
        {
            Intent intent = new Intent(_appliactionContext, typeof(MediaPlayerService.MediaPlayerService));
            intent.SetAction(intentAction);

            PendingIntentFlags flags = PendingIntentFlags.UpdateCurrent;
            if (intentAction.Equals(MediaPlayerService.MediaPlayerService.ActionStop))
                flags = PendingIntentFlags.CancelCurrent;

            PendingIntent pendingIntent = PendingIntent.GetService(_appliactionContext, 1, intent, flags);

            return new Android.Support.V4.App.NotificationCompat.Action.Builder(icon, title, pendingIntent).Build();
        }

        private void AddActionButtons(bool mediaIsPlaying)
        {
            _builder.MActions.Clear();
            _builder.AddAction(GenerateActionCompat(Resource.Drawable.IcMediaPrevious, "Previous", MediaPlayerService.MediaPlayerService.ActionPrevious));
            _builder.AddAction(mediaIsPlaying
                ? GenerateActionCompat(Resource.Drawable.IcMediaPause, "Pause", MediaPlayerService.MediaPlayerService.ActionPause)
                : GenerateActionCompat(Resource.Drawable.IcMediaPlay, "Play", MediaPlayerService.MediaPlayerService.ActionPlay));
            _builder.AddAction(GenerateActionCompat(Resource.Drawable.IcMediaNext, "Next", MediaPlayerService.MediaPlayerService.ActionNext));
        }
    }
}