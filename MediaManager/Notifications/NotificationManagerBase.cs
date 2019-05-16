using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager
{
    public abstract class NotificationManagerBase : INotificationManager
    {
        public bool Enabled { get; set; } = true;
        public bool ShowPlayPauseControls { get; set; } = true;
        public bool ShowNavigationControls { get; set; } = true;

        public abstract void UpdateNotification();
    }
}
