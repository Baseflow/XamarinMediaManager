using System;
using System.Collections.Generic;
using System.Text;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;

namespace MediaManager.Platforms.Android.Audio
{
    public class RatingCallback : Java.Lang.Object, MediaSessionConnector.IRatingCallback
    {
        public RatingCallback(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public RatingCallback()
        {
        }

        public string[] GetCommands()
        {
            throw new NotImplementedException();
        }

        public void OnCommand(IPlayer p0, string p1, Bundle p2, ResultReceiver p3)
        {
            throw new NotImplementedException();
        }

        public void OnSetRating(IPlayer p0, RatingCompat p1)
        {
            throw new NotImplementedException();
        }
    }
}
