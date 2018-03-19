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
        /// True when RenderSurface has been initialized and ready for rendering
        /// </summary>
        bool IsReadyRendering { get; }

        /// <summary>
        /// The aspect mode of the video
        /// </summary>
        VideoAspectMode AspectMode { get; set; }


        bool IsMuted { get; set; }

        /// <summary>
        /// Volume of the video player
        /// </summary>
        /// <param name="leftVolume"></param>
        /// <param name="rightVolume"></param>
        void SetVolume(float leftVolume, float rightVolume);
    }
}