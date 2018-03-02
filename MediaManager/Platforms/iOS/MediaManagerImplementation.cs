using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    public class MediaManagerImplementation: MediaManagerAppleBase
    {
        public MediaManagerImplementation()
        {
            MediaRemoteControl = new MediaRemoteControl(PlaybackController);
            MediaNotificationManager = new MediaNotificationManagerImplementation(this);
        }

        /// <summary>
        /// Default implementation for IMediaRemoteControl that uses the default PlaybackController.
        /// </summary>
        public IMediaRemoteControl MediaRemoteControl { get; set; }

        public sealed override IMediaNotificationManager MediaNotificationManager { get; set; }
    }
}