using System;
using System.ComponentModel;
using AVFoundation;
using Foundation;
using MediaManager.Video;
using UIKit;

namespace MediaManager.Platforms.Ios.Video
{
    [DesignTimeVisible(true)]
    public partial class VideoView : UIView, IVideoView
    {
        private AVPlayerLayer _playerLayer;
        public AVPlayerLayer PlayerLayer
        {
            get => _playerLayer;
            set
            {
                _playerLayer = value;
                this.Layer.AddSublayer(_playerLayer);
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
                switch (PlayerLayer.VideoGravity)
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
                        PlayerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                        break;
                    case VideoAspectMode.AspectFit:
                        PlayerLayer.VideoGravity = AVLayerVideoGravity.Resize;
                        break;
                    case VideoAspectMode.AspectFill:
                        PlayerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
                        break;
                    default:
                        PlayerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                        break;
                }
            }
        }
    }
}
