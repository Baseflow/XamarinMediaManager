using System;
namespace Plugin.MediaManager.Abstractions
{
    public interface IMediaNotificationManager
    {
        void StartNotification(IMediaFile mediaFile);
        void StopNotifications();
    }
}
