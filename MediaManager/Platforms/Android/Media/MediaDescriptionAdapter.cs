using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Runtime;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.UI;
using MediaManager.Library;

namespace MediaManager.Platforms.Android.Media
{
    public class MediaDescriptionAdapter : Java.Lang.Object, PlayerNotificationManager.IMediaDescriptionAdapter
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;

        public MediaDescriptionAdapter()
        {
        }

        protected MediaDescriptionAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public PendingIntent CreateCurrentContentIntent(IPlayer player)
        {
            return MediaManager.SessionActivityPendingIntent;
        }

        public string GetCurrentContentText(IPlayer player)
        {
            return MediaManager.Queue.ElementAtOrDefault(player.CurrentWindowIndex)?.GetTitle();
        }

        public string GetCurrentContentTitle(IPlayer player)
        {
            return MediaManager.Queue.ElementAtOrDefault(player.CurrentWindowIndex)?.GetContentTitle();
        }

        public Bitmap GetCurrentLargeIcon(IPlayer player, PlayerNotificationManager.BitmapCallback callback)
        {
            var mediaItem = MediaManager.Queue.ElementAtOrDefault(player.CurrentWindowIndex);
            if (mediaItem != null)
            {
                Task.Run(async () =>
                {
                    var image = await MediaManager.Extractor.GetMediaImage(mediaItem).ConfigureAwait(false) as Bitmap;
                    callback.OnBitmap(image);
                }).ConfigureAwait(false);
            }
            return null;
        }

        public string GetCurrentSubText(IPlayer player)
        {
            return MediaManager.Queue.ElementAtOrDefault(player.CurrentWindowIndex)?.GetSubText();
        }
    }
}
