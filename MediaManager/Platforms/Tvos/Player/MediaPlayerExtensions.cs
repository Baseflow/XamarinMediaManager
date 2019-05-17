using System;
using AVFoundation;
using MediaManager.Platforms.Tvos.Video;

namespace MediaManager
{
    public static partial class MediaPlayerExtensions
    {
        public static void SetPlayerView(this IMediaPlayer mediaPlayer, VideoView videoView)
        {
            if (mediaPlayer is IMediaPlayer<AVPlayer, VideoView> videoPlayer)
            {
                var playerViewController = new AVKit.AVPlayerViewController();
                playerViewController.Player = videoPlayer.Player;
                videoView.PlayerViewController = playerViewController;
                videoPlayer.PlayerView = videoView;
            }
            else
                throw new ArgumentException($"MediaPlayer needs to be of type {nameof(IMediaPlayer<AVPlayer, VideoView>)} to use this extension", nameof(mediaPlayer));
        }
    }
}
