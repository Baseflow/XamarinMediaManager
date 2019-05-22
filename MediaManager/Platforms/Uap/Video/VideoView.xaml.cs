using System;
using MediaManager.Video;
using Windows.UI.Xaml.Controls;

namespace MediaManager.Platforms.Uap.Video
{
    public sealed partial class VideoView : MediaPlayerElement, IVideoView
    {
        public VideoView()
        {
            this.InitializeComponent();
        }

        public VideoAspectMode VideoAspect
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool ShowControls
        {
            get => AreTransportControlsEnabled;
            set => AreTransportControlsEnabled = value;
        }

        public void Dispose()
        {
        }
    }
}
