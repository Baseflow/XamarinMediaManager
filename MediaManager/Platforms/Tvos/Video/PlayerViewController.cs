using AVKit;

namespace MediaManager.Platforms.Tvos.Video
{
    public class PlayerViewController : AVPlayerViewController
    {
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

            Player = null;
        }
    }
}
