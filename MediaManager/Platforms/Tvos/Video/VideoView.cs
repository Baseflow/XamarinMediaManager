using System;
using System.ComponentModel;
using AVFoundation;
using AVKit;
using CoreGraphics;
using Foundation;
using MediaManager.Video;
using UIKit;

namespace MediaManager.Platforms.Tvos.Video
{
    [DesignTimeVisible(true)]
    public partial class VideoView : UIView, IVideoView
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Apple;

        private AVPlayerViewController _playerViewController;
        public AVPlayerViewController PlayerViewController
        {
            get
            {
                if (_playerViewController == null)
                {
                    PlayerViewController = new AVPlayerViewController();
                }
                return _playerViewController;
            }

            set
            {
                _playerViewController = value;
                _playerViewController.View.Frame = Frame;
                AddSubview(_playerViewController.View);
            }
        }

        public VideoView()
        {
            InitView();
        }

        public VideoView(NSCoder coder) : base(coder)
        {
            InitView();
        }

        public VideoView(CGRect frame) : base(frame)
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
            get
            {
                switch (PlayerViewController.VideoGravity)
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
                        PlayerViewController.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                        break;
                    case VideoAspectMode.AspectFit:
                        PlayerViewController.VideoGravity = AVLayerVideoGravity.Resize;
                        break;
                    case VideoAspectMode.AspectFill:
                        PlayerViewController.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
                        break;
                    default:
                        PlayerViewController.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                        break;
                }
            }
        }

        public bool ShowControls
        {
            get => PlayerViewController.ShowsPlaybackControls;
            set => PlayerViewController.ShowsPlaybackControls = value;
        }

        protected override void Dispose(bool disposing)
        {
            if (MediaManager.MediaPlayer.VideoView == this)
                MediaManager.MediaPlayer.VideoView = null;

            base.Dispose(disposing);
        }
    }
}
