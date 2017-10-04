using System;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager
{
    public class NotificationManagerImplementation : INotificationManager
    {
        private MediaManagerImplementation mediaManagerImplementation;

        public NotificationManagerImplementation(MediaManagerImplementation mediaManagerImplementation)
        {
            this.mediaManagerImplementation = mediaManagerImplementation;
        }

        public void StartNotification(IMediaItem item)
        {
            throw new NotImplementedException();
        }

        public void StopNotifications()
        {
            throw new NotImplementedException();
        }

        public void UpdateNotifications(IMediaItem item, PlaybackState state)
        {
            throw new NotImplementedException();
        }
    }
}
