using System;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Support.V4.App;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    public class MediaNotificationManagerImplementation : IMediaNotificationManager
    {
        public void StartNotification(IMediaFile mediaFile)
        {
            /*if (mediaSessionCompat == null)
                return;

            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, new Intent(Application.Context, typeof(MediaPlayer)), PendingIntentFlags.UpdateCurrent);
            MediaMetadataCompat currentTrack = mediaControllerCompat.Metadata;

            Android.Support.V7.App.NotificationCompat.MediaStyle style = new Android.Support.V7.App.NotificationCompat.MediaStyle();
            style.SetMediaSession(mediaSessionCompat.SessionToken);

            Intent intent = new Intent(Application.Context, typeof(MediaPlayerService));
            intent.SetAction(ActionStop);
            PendingIntent pendingCancelIntent = PendingIntent.GetService(Application.Context, 1, intent, PendingIntentFlags.CancelCurrent);

            style.SetShowCancelButton(true);
            style.SetCancelButtonIntent(pendingCancelIntent);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(Application.Context)
                .SetStyle(style)
                .SetContentTitle(currentTrack.GetString(MediaMetadata.MetadataKeyTitle))
                .SetContentText(currentTrack.GetString(MediaMetadata.MetadataKeyArtist))
                .SetContentInfo(currentTrack.GetString(MediaMetadata.MetadataKeyAlbum))
                .SetSmallIcon(Resource.Drawable.ButtonStar)
                .SetLargeIcon(Cover as Bitmap)
                .SetContentIntent(pendingIntent)
                .SetShowWhen(false)
                .SetOngoing(MediaPlayerState == PlaybackStateCompat.StatePlaying)
                .SetVisibility(NotificationCompat.VisibilityPublic);

            builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaPrevious, "Previous", ActionPrevious));
            AddPlayPauseActionCompat(builder);
            builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaNext, "Next", ActionNext));
            style.SetShowActionsInCompactView(0, 1, 2);

            NotificationManagerCompat.From(Application.Context).Notify(NotificationId, builder.Build());*/
        }

        private NotificationCompat.Action GenerateActionCompat(int icon, String title, String intentAction)
        {
            /*
            Intent intent = new Intent(Application.Context, typeof(MediaPlayerService));
            intent.SetAction(intentAction);

            PendingIntentFlags flags = PendingIntentFlags.UpdateCurrent;
            if (intentAction.Equals(ActionStop))
                flags = PendingIntentFlags.CancelCurrent;

            PendingIntent pendingIntent = PendingIntent.GetService(Application.Context, 1, intent, flags);

            return new NotificationCompat.Action.Builder(icon, title, pendingIntent).Build();*/
        }

        private void AddPlayPauseActionCompat(NotificationCompat.Builder builder)
        {
            /*
            if (MediaPlayerState == PlaybackStateCompat.StatePlaying)
                builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaPause, "Pause", ActionPause));
            else
                builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaPlay, "Play", ActionPlay));
                */
        }

        public void StopNotifications()
        {
            NotificationManagerCompat nm = NotificationManagerCompat.From(Application.Context);
            nm.CancelAll();
        }
    }
}
