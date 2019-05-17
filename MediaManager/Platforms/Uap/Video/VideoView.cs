using System;
using System.Collections.Generic;
using System.Text;
using MediaManager.Video;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaManager.Platforms.Uap.Video
{
    public class VideoView : MediaPlayerElement, IVideoView
    {
        public VideoView()
        {
        }

        public VideoAspectMode VideoAspect {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public bool ShowControls {
            get => AreTransportControlsEnabled;
            set => AreTransportControlsEnabled = value;
        }

        public void Dispose()
        {
        }
    }
}
