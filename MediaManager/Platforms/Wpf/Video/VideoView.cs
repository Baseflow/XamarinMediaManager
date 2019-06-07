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
        private MediaElement _playerView;
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
        }

        public VideoAspectMode VideoAspect { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ShowControls { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
