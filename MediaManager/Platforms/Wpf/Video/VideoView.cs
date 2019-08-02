using System.Windows.Controls;
using MediaManager.Video;

namespace MediaManager.Platforms.Wpf.Video
{
    public class VideoView : UserControl, IVideoView
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Wpf;

        /*private MediaElement _playerView;
        public MediaElement PlayerView
        {
            get
            {
                if (_playerView == null)
                {
                    _playerView = new MediaElement();
                }
                return _playerView;
            }
            set
            {
                _playerView = value;
                Content = _playerView;
            }
        }*/

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

        public VideoAspectMode VideoAspect { get; set; }
        public bool ShowControls { get; set; }

        public void Dispose()
        {
            if (MediaManager.MediaPlayer.AutoAttachVideoView && MediaManager.MediaPlayer.VideoView == this)
                MediaManager.MediaPlayer.VideoView = null;
        }
    }
}
