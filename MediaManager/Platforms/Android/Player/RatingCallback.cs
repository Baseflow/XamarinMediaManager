using System;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;

namespace MediaManager.Platforms.Android.Media
{
    public class RatingCallback : Java.Lang.Object, MediaSessionConnector.IRatingCallback
    {
        public RatingCallback()
        {
        }

        protected RatingCallback(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public string[] GetCommands()
        {
            return null;
        }

        public void OnCommand(IPlayer p0, string p1, Bundle p2, ResultReceiver p3)
        {

        }

        public void OnSetRating(IPlayer player, RatingCompat rating, Bundle extras)
        {
        }

        public void OnSetRating(IPlayer player, RatingCompat rating)
        {

        }
    }
}
