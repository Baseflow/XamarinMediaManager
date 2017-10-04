using System;
using Android.Content;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;

namespace Plugin.MediaManager.Audio
{
    public class MediaBrowserConnectionCallback : MediaBrowserCompat.ConnectionCallback
    {
        private MediaManagerImplementation mediaManagerImplementation;

        public Action OnConnectedImpl { get; set; }

        public Action OnConnectionFailedImpl { get; set; }

        public Action OnConnectionSuspendedImpl { get; set; }

        public MediaBrowserConnectionCallback(MediaManagerImplementation mediaManagerImplementation)
        {
            this.mediaManagerImplementation = mediaManagerImplementation;
        }

        public override void OnConnected()
        {
            base.OnConnected();
            OnConnectedImpl?.Invoke();
        }

        public override void OnConnectionFailed()
        {
            base.OnConnectionFailed();
            OnConnectionFailedImpl?.Invoke();
        }

        public override void OnConnectionSuspended()
        {
            base.OnConnectionSuspended();
            OnConnectionSuspendedImpl?.Invoke();
        }
    }
}
