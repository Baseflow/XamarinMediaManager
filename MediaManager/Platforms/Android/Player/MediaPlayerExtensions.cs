using System;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.UI;
using MediaManager.Platforms.Android.Video;

namespace MediaManager
{
    public static partial class MediaPlayerExtensions
    {
        public static void SetPlayerView(this IMediaPlayer mediaPlayer, VideoView videoView)
        {
            if (mediaPlayer is IMediaPlayer<SimpleExoPlayer, VideoView> videoPlayer)
            {
                videoPlayer.PlayerView = videoView;
                videoView.RequestFocus();
                videoView.Player = videoPlayer.Player;
            }
            else
                throw new ArgumentException($"MediaPlayer needs to be of type {nameof(IMediaPlayer<SimpleExoPlayer, VideoView>)} to use this extension", nameof(IMediaPlayer<SimpleExoPlayer, VideoView>));
        }
    }
}
