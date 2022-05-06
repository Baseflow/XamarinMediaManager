using System.ComponentModel;
using AVKit;
using CoreGraphics;
using Foundation;
using MediaManager.Video;
using UIKit;

namespace MediaManager.Platforms.Ios.Video
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
                    PlayerViewController = new PlayerViewController();
                }
                return _playerViewController;
            }

            set
            {
                _playerViewController = value;
                if (_playerViewController != null)
                {
                    _playerViewController.View.Frame = Bounds;
                    AddSubview(_playerViewController.View);
                    (Superview?.NextResponder as UIViewController)?.AddChildViewController(_playerViewController);
                }
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
    }
}
