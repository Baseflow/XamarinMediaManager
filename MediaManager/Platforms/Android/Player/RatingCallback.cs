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

        public bool OnCommand(IPlayer player, string command, Bundle extras, ResultReceiver cb)
        {
            return false;
        }

        public void OnSetRating(IPlayer player, RatingCompat rating)
        {
        }

        public void OnSetRating(IPlayer player, RatingCompat rating, Bundle extras)
        {
        }
    }
}
