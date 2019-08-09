using MediaManager.Video;
using Windows.UI.Xaml.Controls;

namespace MediaManager.Platforms.Uap.Video
{
    public sealed partial class VideoView : UserControl, IVideoView
    {
        private MediaManagerImplementation MediaManager => CrossMediaManager.Windows;

        private MediaPlayerElement _playerView;
        public MediaPlayerElement PlayerView
        {
            get
            {
                if (_playerView == null)
                {
                    _playerView = new MediaPlayerElement();
                }
                return _playerView;
            }
            set
            {
                _playerView = value;
                Content = _playerView;
            }
        }

        public VideoView()
        {
            this.InitializeComponent();
            Content = PlayerView;
            InitView();
        }

        public void InitView()
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
