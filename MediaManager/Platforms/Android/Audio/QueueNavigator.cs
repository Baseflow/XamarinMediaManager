using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;

namespace MediaManager.Platforms.Android.Audio
{
    public class QueueNavigator : TimelineQueueNavigator
    {
        private Timeline.Window window = new Timeline.Window();
        private MediaSessionCompat mediaSession;
        private IMediaManager mediaManager = CrossMediaManager.Current;

        public QueueNavigator(MediaSessionCompat mediaSession) : base(mediaSession)
        {
            this.mediaSession = mediaSession;
        }

        public QueueNavigator(MediaSessionCompat mediaSession, int maxQueueSize) : base(mediaSession, maxQueueSize)
        {
        }

        public QueueNavigator(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override MediaDescriptionCompat GetMediaDescription(IPlayer player, int windowIndex)
        {
            //var description = player.CurrentTimeline.GetWindow(windowIndex, window, true).Tag as MediaDescriptionCompat;
            var item = mediaManager.MediaQueue.ElementAt(windowIndex).GetMediaDescription();
            return item;
        }
    }
}
