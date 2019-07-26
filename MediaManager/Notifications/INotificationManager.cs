namespace MediaManager.Notifications
{
    public interface INotificationManager
    {
        bool Enabled { get; set; }
        bool ShowPlayPauseControls { get; set; }
        bool ShowNavigationControls { get; set; }

        void UpdateNotification();
    }
}
