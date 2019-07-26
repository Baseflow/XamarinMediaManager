using MediaManager.Video;
using Windows.UI.Xaml.Controls;

namespace MediaManager.Platforms.Uap.Video
{
    public partial class VideoView : UserControl, IVideoView
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Windows;

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

        public virtual void InitView()
        {
            if (MediaManager.MediaPlayer.AutoAttachVideoView)
                MediaManager.MediaPlayer.VideoView = this;
        }

        public VideoAspectMode VideoAspect
        {
            get;
            set;
        }

        public bool ShowControls
        {
            get => PlayerView.AreTransportControlsEnabled;
            set => PlayerView.AreTransportControlsEnabled = value;
        }

        public void Dispose()
        {
            if (MediaManager.MediaPlayer.VideoView == this)
                MediaManager.MediaPlayer.VideoView = null;
        }
    }
}
