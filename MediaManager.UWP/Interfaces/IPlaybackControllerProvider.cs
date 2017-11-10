using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    public interface IPlaybackControllerProvider
    {
        IPlaybackController PlaybackController { get; }
    }
}