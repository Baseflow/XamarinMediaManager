
using System;
using System.Drawing;

using Foundation;
using MediaManager;
using UIKit;

namespace ElementPlayer.iOS.Views
{
    public partial class PlayerViewController : UIViewController
    {
        public PlayerViewController(IntPtr handle) : base(handle)
        {
        }


        #region View lifecycle
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CrossMediaManager.Current.MediaPlayer.SetPlayerView(vwPlayer);
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
