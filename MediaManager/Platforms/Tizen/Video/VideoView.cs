using ElmSharp;
using MediaManager.Video;
using Tizen.Multimedia;

namespace MediaManager.Platforms.Tizen.Video
{
    public class VideoView : MediaView, IVideoView
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Tizen;

        public VideoView(EvasObject parent) : base(parent)
        {
        }

        public virtual void InitView()
        {
            if (MediaManager.MediaPlayer.AutoAttachVideoView)
                MediaManager.MediaPlayer.VideoView = this;
        }

        public void Dispose()
        {
            if (MediaManager.MediaPlayer.VideoView == this)
                MediaManager.MediaPlayer.VideoView = null;
        }
    }
}
