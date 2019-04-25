using System;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Runtime;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.UI;
using MediaManager.Media;

namespace MediaManager.Platforms.Android.Media
{
    public class MediaDescriptionAdapter : Java.Lang.Object, PlayerNotificationManager.IMediaDescriptionAdapter
    {
        private PendingIntent sessionActivityPendingIntent;
        private IMediaManager mediaManager = CrossMediaManager.Current;

        public MediaDescriptionAdapter(PendingIntent sessionActivityPendingIntent)
        {
            this.sessionActivityPendingIntent = sessionActivityPendingIntent;
        }

        public MediaDescriptionAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public PendingIntent CreateCurrentContentIntent(IPlayer player)
        {
            return sessionActivityPendingIntent;
        }

        public string GetCurrentContentText(IPlayer player)
        {
            return mediaManager.MediaQueue.ElementAtOrDefault(player.CurrentWindowIndex)?.GetTitle();
        }

        public string GetCurrentContentTitle(IPlayer player)
        {
            return mediaManager.MediaQueue.ElementAtOrDefault(player.CurrentWindowIndex)?.GetContentTitle();
        }

        public Bitmap GetCurrentLargeIcon(IPlayer player, PlayerNotificationManager.BitmapCallback callback)
        {
            return mediaManager.MediaQueue.ElementAtOrDefault(player.CurrentWindowIndex)?.GetCover();
        }
    }
}
