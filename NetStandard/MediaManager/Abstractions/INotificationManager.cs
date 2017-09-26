using MediaManager.Abstractions.Enums;

namespace MediaManager.Abstractions
{
    /// <summary>
    /// Manages the notifications to the native platform
    /// </summary>
    public interface INotificationManager
    {
        /// <summary>
        /// Starts the notification.
        /// </summary>
        /// <param name="mediaFile">The media file.</param>
        void StartNotification(IMediaItem mediaFile);

        /// <summary>
        /// Stops the notifications.
        /// </summary>
        void StopNotifications();

        /// <summary>
        /// Updates the notifications.
        /// </summary>
        /// <param name="mediaFile">The media file.</param>
        /// <param name="status">The status.</param>
        void UpdateNotifications(IMediaItem mediaFile, PlaybackState status);
    }
}
