using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    /// <summary>
    ///     Implementation for MediaManager
    /// </summary>
    public class MediaManagerImplementation : MediaManagerBase
    {
        public MediaManagerImplementation()
        {
            Init();
        }
        public override IAudioPlayer AudioPlayer { get; } = new AudioPlayerImplementation();
        public override IVideoPlayer VideoPlayer { get; }
        public override IMediaQueue MediaQueue { get; }
        public override IMediaNotificationManager MediaNotificationManager { get; }
        public override IMediaExtractor MediaExtractor { get; }
    }
}