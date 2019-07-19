using System;

namespace MediaManager.Video
{
    public interface IVideoView : IDisposable
    {
        void InitView();

        VideoAspectMode VideoAspect { get; set; }

        bool ShowControls { get; set; }
    }
}
