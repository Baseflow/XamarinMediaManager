using System.ComponentModel;
using AVFoundation;
using Foundation;
using MediaManager.Video;
using UIKit;

namespace MediaManager.Platforms.Ios.Video
{
    [DesignTimeVisible(true)]
    public class VideoView : AVPlayerLayer, IVideoView
    {
        [Export("VideoAspect"), Browsable(true)]
        public VideoAspectMode VideoAspect { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}
