namespace Plugin.MediaManager
{
    public class MediaManagerImplementation : MediaManagerAppleBase
    {
        /// <summary>
        /// Default implementation for IMediaRemoteControl that uses the default PlaybackController.
        /// </summary>
        public IMediaRemoteControl MediaRemoteControl => new MediaRemoteControl(PlaybackController);
    }
}
