
using System;
using System.Drawing;

using Foundation;
using MediaManager;
using MediaManager.Platforms.Ios.Video;
using UIKit;

namespace ElementPlayer.iOS.Views
{
    public partial class PlayerViewController : UIViewController
    {
        VideoSurface _videoSurface;

        public PlayerViewController(IntPtr handle) : base(handle)
        {
        }


        #region View lifecycle
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _videoSurface = new VideoSurface();
            vwPlayer.Add(_videoSurface);
            CrossMediaManager.Current.MediaPlayer.SetPlayerView(_videoSurface);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        #endregion
    }
}
