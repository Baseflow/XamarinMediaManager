using System;
using Android.OS;
using Android.Runtime;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;

namespace MediaManager.Platforms.Android.Media
{
    public class PlaybackController : DefaultPlaybackController
    {
        public PlaybackController()
        {
        }

        protected PlaybackController(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public PlaybackController(long rewindIncrementMs, long fastForwardIncrementMs, int repeatToggleModes) : base(rewindIncrementMs, fastForwardIncrementMs, repeatToggleModes)
        {
        }
    }
}
