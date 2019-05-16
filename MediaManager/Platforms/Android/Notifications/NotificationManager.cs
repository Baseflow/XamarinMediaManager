using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager.Platforms.Android.Notifications
{
    public class NotificationManager : NotificationManagerBase
    {
        public override void UpdateNotification()
        {
            //TODO: Call PlayerNotificationManager.Invalidate(); on exoplayer 2.9.6 when metadata is updated
        }
    }
}
