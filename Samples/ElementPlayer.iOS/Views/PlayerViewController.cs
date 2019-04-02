using System;
using ElementPlayer.Core.ViewModels;
using MediaManager;
using MediaManager.Media;
using MediaManager.Platforms.Ios.Video;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace ElementPlayer.iOS.Views
{
    [MvxRootPresentation(WrapInNavigationController = false)]
    [MvxFromStoryboard]
    public partial class PlayerViewController : MvxViewController<PlayerViewModel>
    {
        private VideoSurface _videoSurface;

        public PlayerViewController(IntPtr handle) : base(handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _videoSurface = new VideoSurface();
            _videoSurface.Frame = vwPlayer.Frame;
            vwPlayer.Add(_videoSurface);
            CrossMediaManager.Current.MediaPlayer.SetPlayerView(_videoSurface);
        }

        partial void UIButton1265_TouchUpInside(UIButton sender)
        {
            var video = new MediaItem("http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4");
            CrossMediaManager.Current.Play(video);
        }
    }
}

