using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MediaManager.Video;

namespace MediaManager.Platforms.Wpf.Video
{
    public class VideoView : UserControl, IVideoView
    {
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

        protected MediaManagerImplementation MediaManager = CrossMediaManager.Wpf;

        public VideoView()
        {
            Content = MediaManager.WpfMediaPlayer.Player;
        }

        public VideoAspectMode VideoAspect { get; set; }
        public bool ShowControls { get; set; }

        public void Dispose()
        {

        }
    }
}
