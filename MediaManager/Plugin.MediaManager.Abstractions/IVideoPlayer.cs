using System;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager.Abstractions
{
    public interface IVideoPlayer : IPlaybackManager
    {
        IVideoSurface RenderSurface { get; set; }
        VideoAspectMode AspectMode { get; set; }
    }
}