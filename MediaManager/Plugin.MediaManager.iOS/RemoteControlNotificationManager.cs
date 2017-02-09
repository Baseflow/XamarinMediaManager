using Foundation;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using UIKit;

namespace Plugin.MediaManager
{
    public class RemoteControlNotificationManager: NSObject, IMediaNotificationManager
    {
        public virtual void StartNotification(IMediaFile mediaFile)
        {
            InvokeOnMainThread(() => {
                UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
            });
        }

        public virtual void StopNotifications()
        {
            InvokeOnMainThread(() =>
            {
                UIApplication.SharedApplication.EndReceivingRemoteControlEvents();
            });
        }

        public virtual void UpdateNotifications(IMediaFile mediaFile, MediaPlayerStatus status)
        {
        }
    }
}