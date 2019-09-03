namespace MediaManager.Notifications
{
    public abstract class NotificationManagerBase : NotifyPropertyChangedBase, INotificationManager
    {
        private bool _enabled = true;
        private bool _showPlayPauseControls = true;
        private bool _showNavigationControls = true;

        public virtual bool Enabled
        {
            get => _enabled;
            set
            {
                if (SetProperty(ref _enabled, value))
                    UpdateNotification();
            }
        }

        public virtual bool ShowPlayPauseControls
        {
            get => _showPlayPauseControls;
            set
            {
                if (SetProperty(ref _showPlayPauseControls, value))
                    UpdateNotification();
            }
        }

        public virtual bool ShowNavigationControls
        {
            get => _showNavigationControls;
            set
            {
                if (SetProperty(ref _showNavigationControls, value))
                    UpdateNotification();
            }
        }

        public abstract void UpdateNotification();
    }
}
