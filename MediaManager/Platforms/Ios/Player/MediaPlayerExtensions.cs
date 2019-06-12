using System;
using AVFoundation;
using MediaManager.Platforms.Ios.Video;

namespace MediaManager
{
    public static partial class MediaPlayerExtensions
    {
        public static void SetPlayerView(this IMediaPlayer mediaPlayer, VideoView videoView)
        {
            if (mediaPlayer is IMediaPlayer<AVQueuePlayer, VideoView> videoPlayer)
            {
                videoView.PlayerViewController.Player = videoPlayer.Player;
                videoPlayer.PlayerView = videoView;
            }
            else
                throw new ArgumentException($"MediaPlayer needs to be of type {nameof(IMediaPlayer<AVQueuePlayer, VideoView>)} to use this extension", nameof(mediaPlayer));
        }
    }
}
