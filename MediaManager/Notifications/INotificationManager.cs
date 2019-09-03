using System.ComponentModel;

namespace MediaManager.Notifications
{
    public interface INotificationManager : INotifyPropertyChanged
    {
        bool Enabled { get; set; }
        bool ShowPlayPauseControls { get; set; }
        bool ShowNavigationControls { get; set; }

        void UpdateNotification();
    }
}
