using System;
namespace Plugin.MediaManager.Abstractions
{
    public interface IVideoPlayer : IPlaybackManager
    {
        IVideoSurface RenderSurface { get; }
        void SetVideoSurface(IVideoSurface videoSurface);
    }
}