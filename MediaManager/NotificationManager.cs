using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager
{
    public class NotificationManager : INotificationManager
    {
        public bool Enabled { get; set; } = true;
        public bool ShowPlayPauseControls { get; set; } = true;
        public bool ShowNavigationControls { get; set; } = true;
    }
}
