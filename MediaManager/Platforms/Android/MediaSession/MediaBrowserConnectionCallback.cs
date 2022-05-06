using Android.Runtime;
using Android.Support.V4.Media;

namespace MediaManager.Platforms.Android.MediaSession
{
    public class MediaBrowserConnectionCallback : MediaBrowserCompat.ConnectionCallback
    {
        public MediaBrowserConnectionCallback()
        {
        }

        protected MediaBrowserConnectionCallback(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public Action OnConnectedImpl { get; set; }

        public Action OnConnectionFailedImpl { get; set; }

        public Action OnConnectionSuspendedImpl { get; set; }

        public override void OnConnected()
        {
            OnConnectedImpl?.Invoke();
        }

        public override void OnConnectionFailed()
        {
            OnConnectionFailedImpl?.Invoke();
        }

        public override void OnConnectionSuspended()
        {
            OnConnectionSuspendedImpl?.Invoke();
        }
    }
}
