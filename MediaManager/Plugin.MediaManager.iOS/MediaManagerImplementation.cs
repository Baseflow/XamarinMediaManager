using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    /// <summary>
    ///     Implementation for MediaManager
    /// </summary>
    public class MediaManagerImplementationIOS : MediaManagerBase
    {
        public MediaManagerImplementationIOS()
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