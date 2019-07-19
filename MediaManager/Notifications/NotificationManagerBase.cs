using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager
{
    public abstract class NotificationManagerBase : INotificationManager
    {
        protected NotificationManagerBase()
        {
            Enabled = true;
            ShowPlayPauseControls = true;
            ShowNavigationControls = true;
        }
        
        private bool _enabled;
        private bool _showPlayPauseControls;
        private bool _showNavigationControls;

        public virtual bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                UpdateNotification();
            }
        }

        public virtual bool ShowPlayPauseControls
        {
            get => _showPlayPauseControls;
            set
            {
                _showPlayPauseControls = value;
                UpdateNotification();
            }
        }

        public virtual bool ShowNavigationControls
        {
            get => _showNavigationControls;
            set
            {
                _showNavigationControls = value;
                UpdateNotification();
            }
        }

        public abstract void UpdateNotification();
    }
}
