using System;
using MediaManager.Video;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Controls;

namespace MediaManager.Platforms.Uap.Video
{
    public sealed partial class VideoView : UserControl, IVideoView
    {
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
        }
    }
}
