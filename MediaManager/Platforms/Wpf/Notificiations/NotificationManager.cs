using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager.Platforms.Wpf.Notificiations
{
    public class NotificationManager : INotificationManager
    {
        public bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ShowPlayPauseControls { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ShowNavigationControls { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void UpdateNotification()
        {
            throw new NotImplementedException();
        }
    }
}
