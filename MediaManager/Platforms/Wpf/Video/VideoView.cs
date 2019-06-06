using System;
using System.Collections.Generic;
using System.Text;
using MediaManager.Video;

namespace MediaManager.Platforms.Wpf.Video
{
    public class VideoView : IVideoView
    {
        public VideoAspectMode VideoAspect { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ShowControls { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
