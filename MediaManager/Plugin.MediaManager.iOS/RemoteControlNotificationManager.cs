using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using UIKit;

namespace Plugin.MediaManager
{
    public class RemoteControlNotificationManager: IMediaNotificationManager
    {
        public virtual void StartNotification(IMediaFile mediaFile)
        {
            UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
        }

        public virtual void StopNotifications()
        {
            UIApplication.SharedApplication.EndReceivingRemoteControlEvents();
        }

        public virtual void UpdateNotifications(IMediaFile mediaFile, MediaPlayerStatus status)
        {
        }
    }
}