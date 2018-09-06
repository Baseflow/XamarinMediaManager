using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Graphics;
using Android.Runtime;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.UI;
using static Com.Google.Android.Exoplayer2.Trackselection.MappingTrackSelector;

namespace MediaManager.Platforms.Android.Audio
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

        protected MediaDescriptionAdapter()
        {
        }

        public PendingIntent CreateCurrentContentIntent(IPlayer player)
        {
            return sessionActivityPendingIntent;
        }

        public string GetCurrentContentText(IPlayer player)
        {
            return mediaManager.MediaQueue.ElementAtOrDefault(player.CurrentWindowIndex)?.Title;
        }

        public string GetCurrentContentTitle(IPlayer player)
        {
            return mediaManager.MediaQueue.ElementAtOrDefault(player.CurrentWindowIndex)?.AlbumArtist;
        }



        public Bitmap GetCurrentLargeIcon(IPlayer player, PlayerNotificationManager.BitmapCallback callback)
        {
            return mediaManager.MediaQueue.ElementAtOrDefault(player.CurrentWindowIndex)?.AlbumArt as Bitmap;
        }
    }
}
