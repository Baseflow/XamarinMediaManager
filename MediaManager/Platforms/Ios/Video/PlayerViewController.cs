using AVKit;
using CoreGraphics;
using Foundation;

namespace MediaManager.Platforms.Ios.Video
{
    public class PlayerViewController : AVPlayerViewController
    {
        private bool keepPlayer;

        public PlayerViewController()
        {
            this.AddObserver(this, "contentOverlayView.frame", NSKeyValueObservingOptions.OldNew, Handle);
        }

        protected static MediaManagerImplementation MediaManager => CrossMediaManager.Apple;

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
            if (!keepPlayer)
                Player = null;
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, System.IntPtr context)
        {

            if (keyPath == "contentOverlayView.frame" && change != null && change[ChangeOldKey] != null)
            {
                var oldNsValue = change[ChangeOldKey];
                var newNsValue = change[ChangeNewKey];
                if (oldNsValue != null && newNsValue != null)
                {
                    var oldValue = (oldNsValue as NSValue).CGRectValue;
                    var newValue = (newNsValue as NSValue).CGRectValue;
                    if (oldValue != CGRect.Empty && oldValue != CGRect.Null && !oldValue.Equals(newValue))
                    {
                        //if portrait
                        if (UIKit.UIScreen.MainScreen.Bounds.Height > UIKit.UIScreen.MainScreen.Bounds.Width)
                        {
                            if (newValue.Height < oldValue.Height)
                            {
                                //normalScreen so you can empty the player 
                                keepPlayer = false;
                            }
                            else
                            {
                                //fullScreen so you need to keep player.
                                keepPlayer = true;
                            }
                        }
                        else
                        {
                            if (newValue.Width < UIKit.UIScreen.MainScreen.Bounds.Width)
                            {
                                keepPlayer = false;
                            }
                            else
                            {
                                keepPlayer = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
