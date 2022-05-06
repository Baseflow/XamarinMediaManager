using AVKit;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MediaManager.Platforms.Ios.Video
{
    public class PlayerViewController : AVPlayerViewController
    {
        private const string KeyPath = "contentOverlayView.frame";

        private bool _savePlayer;

        protected static MediaManagerImplementation MediaManager => CrossMediaManager.Apple;

        public PlayerViewController()
        {
            AddObserver(this, KeyPath, NSKeyValueObservingOptions.OldNew, Handle);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (MediaManager.MediaPlayer.AutoAttachVideoView && View.Superview is VideoView videoView)
                MediaManager.MediaPlayer.VideoView = videoView;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (MediaManager.MediaPlayer.AutoAttachVideoView && MediaManager.MediaPlayer.VideoView == View.Superview)
            {
                MediaManager.MediaPlayer.VideoView = null;
            }

            if (!_savePlayer)
            {
                Player = null;
            }
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if (keyPath != KeyPath || change == null || change[ChangeOldKey] == null) return;

            var oldNsValue = change[ChangeOldKey];
            var newNsValue = change[ChangeNewKey];

            if (oldNsValue == null || newNsValue == null) return;

            var oldValue = ((NSValue)oldNsValue).CGRectValue;
            var newValue = ((NSValue)newNsValue).CGRectValue;

            if (oldValue == CGRect.Empty || oldValue == CGRect.Null || oldValue.Equals(newValue)) return;

            if (UIScreen.MainScreen.Bounds.Height > UIScreen.MainScreen.Bounds.Width)
            {
                _savePlayer = newValue.Y >= oldValue.Y;
            }
            else
            {
                _savePlayer = newValue.Height >= UIScreen.MainScreen.Bounds.Height;
            }
        }
    }
}
