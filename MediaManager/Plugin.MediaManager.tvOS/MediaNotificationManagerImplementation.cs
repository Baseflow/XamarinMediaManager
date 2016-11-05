using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class MediaNotificationManagerImplementation : IMediaNotificationManager
    {
        public void StartNotification(IMediaFile mediaFile)
        {
            
        }

        public void StopNotifications()
        {
        }

        public void UpdateNotifications(IMediaFile mediaFile, MediaPlayerStatus status)
        {
        }
    }
}
