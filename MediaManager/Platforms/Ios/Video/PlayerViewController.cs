using AVKit;

namespace MediaManager.Platforms.Ios.Video
{
    public class PlayerViewController : AVPlayerViewController
    {
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            Player = null;
        }
    }
}
