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

        public Action<int, bool> OnNotificationCancelledImpl { get; set; }
        public Action<int, Notification> OnNotificationStartedImpl { get; set; }
        public Action<int, Notification, bool> OnNotificationPostedImpl { get; set; }

        public void OnNotificationCancelled(int p0, bool p1) => OnNotificationCancelledImpl?.Invoke(p0, p1);
        public void OnNotificationStarted(int p0, Notification p1) => OnNotificationStartedImpl?.Invoke(p0, p1);
        public void OnNotificationPosted(int p0, Notification p1, bool p2) => OnNotificationPostedImpl?.Invoke(p0, p1, p2);
    }
}
