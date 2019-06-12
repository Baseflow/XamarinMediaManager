using System;
using System.ComponentModel;
using AVFoundation;
using AVKit;
using Foundation;
using MediaManager.Video;
using UIKit;

namespace MediaManager.Platforms.Ios.Video
{
    [DesignTimeVisible(true)]
    public partial class VideoView : UIView, IVideoView
    {
        private AVPlayerViewController _playerViewController;
        public AVPlayerViewController PlayerViewController
        {
            get
            {
                if(_playerViewController == null)
                {
                    PlayerViewController = new AVPlayerViewController();
                }
                return _playerViewController;
            }

            set
            {
                _playerViewController = value;
                if (_playerViewController != null)
                {
                    _playerViewController.View.Frame = Frame;
                    AddSubview(_playerViewController.View);
                }
            }
        }

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
                        PlayerViewController.VideoGravity = AVLayerVideoGravity.Resize;
                        break;
                    case VideoAspectMode.AspectFit:
                        PlayerViewController.VideoGravity = AVLayerVideoGravity.ResizeAspect;
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

        public bool ShowControls {
            get => PlayerViewController.ShowsPlaybackControls;
            set => PlayerViewController.ShowsPlaybackControls = value;
        }

        protected override void Dispose(bool disposing)
        {
            //Cleanup sublayars because otherwise video doesn't work a second time
            PlayerViewController.View.Layer.Sublayers = null;
            PlayerViewController = null;
            base.Dispose(disposing);
        }
    }
}
