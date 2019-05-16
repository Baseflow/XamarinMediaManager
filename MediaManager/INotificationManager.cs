namespace MediaManager
{
    public interface INotificationManager
    {
        bool Enabled { get; set; }
        bool ShowPlayPauseControls { get; set; }
        bool ShowNavigationControls { get; set; }
    }
}
