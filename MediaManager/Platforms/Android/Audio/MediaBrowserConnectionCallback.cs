using System;
using System.Collections.Generic;
using System.Text;
using Android.Support.V4.Media;

namespace MediaManager.Platforms.Android.Audio
{
    public class MediaBrowserConnectionCallback : MediaBrowserCompat.ConnectionCallback
    {
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
