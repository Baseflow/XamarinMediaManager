using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;

namespace MediaManager.Platforms.Android.Player
{
    public class RatingCallback : Java.Lang.Object, MediaSessionConnector.IRatingCallback
    {
        public RatingCallback()
        {
        }

        protected RatingCallback(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public bool OnCommand(IPlayer p0, IControlDispatcher p1, string p2, Bundle p3, ResultReceiver p4)
        {
            return false;
        }

        public void OnSetRating(IPlayer p0, RatingCompat p1)
        {
        }

        public void OnSetRating(IPlayer p0, RatingCompat p1, Bundle p2)
        {
        }
    }
}
