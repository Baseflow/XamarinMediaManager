using Foundation;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using UIKit;

namespace Plugin.MediaManager
{
    public class RemoteControlNotificationManager: NSObject, INotificationManager
    {
        public virtual void StartNotification(IMediaItem mediaFile)
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

        public virtual void UpdateNotifications(IMediaItem mediaFile, PlaybackState status)
        {
        }
    }
}