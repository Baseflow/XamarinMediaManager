using System;
using System.ComponentModel;
using AVFoundation;
using AVKit;
using Foundation;
using MediaManager.Video;
using UIKit;

namespace MediaManager.Platforms.Tvos.Video
{
    [DesignTimeVisible(true)]
    public partial class VideoView : AVPlayerViewController, IVideoView
    {
        public VideoView()
        {
        }

        public VideoView(NSCoder coder) : base(coder)
        {
        }

        public VideoView(IntPtr handle) : base(handle)
        {
        }

        protected VideoView(NSObjectFlag t) : base(t)
        {
        }

        [Export("VideoAspect"), Browsable(true)]
        public VideoAspectMode VideoAspect
        {
            get
            {
                switch (VideoGravity)
                {
                    case AVLayerVideoGravity.ResizeAspect:
                        return VideoAspectMode.None;
                    case AVLayerVideoGravity.ResizeAspectFill:
                        return VideoAspectMode.AspectFill;
                    case AVLayerVideoGravity.Resize:
                        return VideoAspectMode.AspectFit;
                    default:
                        return VideoAspectMode.None;
                }
            }

            set
            {
                switch (value)
                {
                    case VideoAspectMode.None:
                        VideoGravity = AVLayerVideoGravity.ResizeAspect;
                        break;
                    case VideoAspectMode.AspectFit:
                        VideoGravity = AVLayerVideoGravity.Resize;
                        break;
                    case VideoAspectMode.AspectFill:
                        VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
                        break;
                    default:
                        VideoGravity = AVLayerVideoGravity.ResizeAspect;
                        break;
                }
            }
        }

        public bool ShowControls
        {
            get => ShowsPlaybackControls;
            set => ShowsPlaybackControls = value;
        }
    }
}
