using System.Windows.Controls;
using MediaManager.Video;

namespace MediaManager.Platforms.Wpf.Video
{
    public class VideoView : UserControl, IVideoView
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Wpf;

        public VideoView()
        {
            Content = MediaManager.Player;
            InitView();
        }

        public virtual void InitView()
        {
            if (MediaManager.MediaPlayer.AutoAttachVideoView)
                MediaManager.MediaPlayer.VideoView = this;
        }

        public void Dispose()
        {
            if (MediaManager.MediaPlayer.AutoAttachVideoView && MediaManager.MediaPlayer.VideoView == this)
                MediaManager.MediaPlayer.VideoView = null;
        }
    }
}
