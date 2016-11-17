namespace Plugin.MediaManager.Abstractions
{

    using Plugin.MediaManager.Abstractions.Enums;
    using Plugin.MediaManager.Abstractions.Implementations;

    /// <summary>
    /// Manages the notifications to the native platform
    /// </summary>
    public interface IMediaNotificationManager
    {
        /// <summary>
        /// Starts the notification.
        /// </summary>
        /// <param name="mediaFile">The media file.</param>
        void StartNotification(IMediaFile mediaFile);

        /// <summary>
        /// Stops the notifications.
        /// </summary>
        void StopNotifications();

        /// <summary>
        /// Updates the notifications.
        /// </summary>
        /// <param name="mediaFile">The media file.</param>
        /// <param name="status">The status.</param>
        void UpdateNotifications(IMediaFile mediaFile, MediaPlayerStatus status);
    }
}