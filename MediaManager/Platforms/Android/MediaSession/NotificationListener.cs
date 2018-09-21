using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Com.Google.Android.Exoplayer2.UI;

namespace MediaManager.Platforms.Android.MediaSession
{
    public class NotificationListener : Java.Lang.Object, PlayerNotificationManager.INotificationListener
    {
        public void OnNotificationCancelled(int p0)
        {
        }

        public void OnNotificationStarted(int p0, Notification p1)
        {
        }
    }
}
