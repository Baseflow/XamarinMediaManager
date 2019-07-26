using AVKit;

namespace MediaManager.Platforms.Ios.Video
{
    public class PlayerViewController : AVPlayerViewController
    {
        protected static MediaManagerImplementation MediaManager => CrossMediaManager.Apple;

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (MediaManager.MediaPlayer.VideoView == View.Superview)
            {
                MediaManager.MediaPlayer.VideoView = null;
            }

            Player = null;
        }
    }
}
