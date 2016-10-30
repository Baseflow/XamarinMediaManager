using System;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager.Abstractions
{
    public interface IVideoPlayer : IPlaybackManager
    {
        IVideoSurface RenderSurface { get; }
        void SetVideoSurface(IVideoSurface videoSurface);
        VideoAspectMode AspectMode { get; }
        void SetAspectMode(VideoAspectMode aspectMode);
    }
}