using System;

namespace MediaManager.Video
{
    public interface IVideoView : IDisposable
    {
        VideoAspectMode VideoAspect { get; set; }
    }
}
