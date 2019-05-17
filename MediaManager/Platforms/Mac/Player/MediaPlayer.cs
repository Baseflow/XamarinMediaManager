using AppKit;
using AVFoundation;
using MediaManager.Platforms.Apple.Media;
using MediaManager.Platforms.Mac.Video;
using MediaManager.Video;

namespace MediaManager.Platforms.Mac.Media
{
    public class MediaPlayer : AppleMediaPlayer, IMediaPlayer<AVPlayer, VideoView>
    {
        public VideoView PlayerView { get; set; }

        public override IVideoView VideoView => PlayerView;
    }
}
