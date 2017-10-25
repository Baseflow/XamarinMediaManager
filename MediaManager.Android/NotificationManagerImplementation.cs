using System;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager
{
    public class NotificationManagerImplementation : INotificationManager
    {
        private MediaManagerImplementation _mediaManagerImplementation;

        public NotificationManagerImplementation(MediaManagerImplementation mediaManagerImplementation)
        {
            _mediaManagerImplementation = mediaManagerImplementation;
        }

        public void StartNotification(IMediaItem item)
        {
            //TODO: StartNotification
        }

        public void StopNotifications()
        {
            //TODO: StopNotification
        }

        public void UpdateNotifications(IMediaItem item, PlaybackState state)
        {
            //TODO: UpdateNotification
        }
    }
}
