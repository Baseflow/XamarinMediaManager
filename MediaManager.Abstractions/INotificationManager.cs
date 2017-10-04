using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager.Abstractions
{
    /// <summary>
    /// Manages the notifications to the native platform
    /// </summary>
    public interface INotificationManager
    {
        /// <summary>
        /// Starts the notification.
        /// </summary>
        /// <param name="item">The media file.</param>
        void StartNotification(IMediaItem item);

        /// <summary>
        /// Stops the notifications.
        /// </summary>
        void StopNotifications();

        /// <summary>
        /// Updates the notifications.
        /// </summary>
        /// <param name="item">The media file.</param>
        /// <param name="state">The status.</param>
        void UpdateNotifications(IMediaItem item, PlaybackState state);
    }
}