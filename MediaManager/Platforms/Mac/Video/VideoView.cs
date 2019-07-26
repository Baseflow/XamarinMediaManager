using System;
using System.ComponentModel;
using AVKit;
using CoreGraphics;
using Foundation;
using MediaManager.Video;

namespace MediaManager.Platforms.Mac.Video
{
    public class VideoView : AVPlayerView, IVideoView
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Apple;

        public VideoView()
        {
            InitView();
        }

        public VideoView(NSCoder coder) : base(coder)
        {
            InitView();
        }

        public VideoView(CGRect frameRect) : base(frameRect)
        {
            InitView();
        }

        protected VideoView(NSObjectFlag t) : base(t)
        {
        }

        protected internal VideoView(IntPtr handle) : base(handle)
        {
        }

        public virtual void InitView()
        {
            if (MediaManager.MediaPlayer.AutoAttachVideoView)
                MediaManager.MediaPlayer.VideoView = this;
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

        protected override void Dispose(bool disposing)
        {
            if (MediaManager.MediaPlayer.VideoView == this)
                MediaManager.MediaPlayer.VideoView = null;

            base.Dispose(disposing);
        }
    }
}
