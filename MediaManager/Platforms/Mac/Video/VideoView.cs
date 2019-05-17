using System;
using System.ComponentModel;
using AppKit;
using AVFoundation;
using AVKit;
using CoreGraphics;
using Foundation;
using MediaManager.Video;

namespace MediaManager.Platforms.Mac.Video
{
    public class VideoView : AVPlayerView, IVideoView
    {
        public VideoView()
        {
        }

        public VideoView(NSCoder coder) : base(coder)
        {
        }

        public VideoView(CGRect frameRect) : base(frameRect)
        {
        }

        protected VideoView(NSObjectFlag t) : base(t)
        {
        }

        protected internal VideoView(IntPtr handle) : base(handle)
        {
        }

        [Export("VideoAspect"), Browsable(true)]
        public VideoAspectMode VideoAspect
        {
            get;
            set;
        }

        public bool ShowControls
        {
            get
            {
                if (ControlsStyle == AVPlayerViewControlsStyle.Default)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                    ControlsStyle = AVPlayerViewControlsStyle.Default;
                else
                    ControlsStyle = AVPlayerViewControlsStyle.None;
            }
        }
    }
}
