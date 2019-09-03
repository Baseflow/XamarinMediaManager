using System;
using System.Linq;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using MediaManager.Platforms.Android.Media;

namespace MediaManager.Platforms.Android.Queue
{
    public class QueueNavigator : TimelineQueueNavigator
    {
        protected MediaSessionCompat _mediaSession;
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;

        public QueueNavigator(MediaSessionCompat mediaSession) : base(mediaSession)
        {
            _mediaSession = mediaSession;
        }

        public QueueNavigator(MediaSessionCompat mediaSession, int maxQueueSize) : base(mediaSession, maxQueueSize)
        {
        }

        protected QueueNavigator(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override MediaDescriptionCompat GetMediaDescription(IPlayer player, int windowIndex)
        {
            return MediaManager.Queue.ElementAtOrDefault(windowIndex)?.ToMediaDescription();
        }
    }
}
