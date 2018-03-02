using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    internal interface IPlaybackControllerProvider
    {
        IPlaybackController PlaybackController { get; }
    }
}