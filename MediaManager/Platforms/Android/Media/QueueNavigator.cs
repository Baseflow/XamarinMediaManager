using System;
using System.Linq;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;

namespace MediaManager.Platforms.Android.Media
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

        protected QueueNavigator(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override MediaDescriptionCompat GetMediaDescription(IPlayer player, int windowIndex)
        {
            return mediaManager.MediaQueue.ElementAtOrDefault(windowIndex)?.ToMediaDescription();
        }
    }
}
