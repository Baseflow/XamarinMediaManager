using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager.Abstractions
{
    /// <summary>
    /// Plays the video
    /// </summary>
    public interface IVideoPlayer : IPlaybackManager
    {
        /// <summary>
        /// The native view where the video should be rendered on
        /// </summary>
        IVideoSurface RenderSurface { get; set; }

        /// <summary>
        /// The aspect mode of the video
        /// </summary>
        VideoAspectMode AspectMode { get; set; }
    }
}