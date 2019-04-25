using System;
using Android.App;
using Android.Runtime;
using Com.Google.Android.Exoplayer2.UI;

namespace MediaManager.Platforms.Android.MediaSession
{
    public class NotificationListener : Java.Lang.Object, PlayerNotificationManager.INotificationListener
    {
        public NotificationListener()
        {
        }

        protected NotificationListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public Action<int> OnNotificationCancelledImpl { get; set; }
        public Action<int, Notification> OnNotificationStartedImpl { get; set; }

        public void OnNotificationCancelled(int p0)
        {
            OnNotificationCancelledImpl?.Invoke(p0);
        }

        public void OnNotificationStarted(int p0, Notification p1)
        {
            OnNotificationStartedImpl?.Invoke(p0, p1);
        }
    }
}
