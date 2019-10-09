using System;

namespace MediaManager.Video
{
    public interface IVideoView : IDisposable
    {
        void InitView();

        object VideoPlaceholder { get; set; }
    }
}
