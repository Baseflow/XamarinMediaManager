using AppKit;
using AVFoundation;
using MediaManager.Platforms.Apple.Media;
using MediaManager.Platforms.Apple.Player;
using MediaManager.Platforms.Mac.Video;
using MediaManager.Player;
using MediaManager.Video;

namespace MediaManager.Platforms.Mac.Player
{
    public class MacMediaPlayer : AppleMediaPlayer, IMediaPlayer<AVQueuePlayer, VideoView>
    {
        public VideoView PlayerView => VideoView as VideoView;

        private IVideoView _videoView;
        public override IVideoView VideoView
        {
            get => _videoView;
            set
            {
                _videoView = value;
                if (PlayerView != null)
                {
                    PlayerView.Player = Player;
                }
            }
        }
    }
}
