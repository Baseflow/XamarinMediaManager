using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using MediaManager.Platforms.Wpf.Video;

namespace MediaManager
{
    public static partial class MediaPlayerExtensions
    {
        public static void SetPlayerView(this IMediaPlayer mediaPlayer, VideoView videoView)
        {
            if (mediaPlayer is IMediaPlayer<MediaPlayer, VideoView> videoPlayer)
            {
                //videoView.PlayerView.SetMediaPlayer(videoPlayer.Player);
                videoPlayer.PlayerView = videoView;
            }
            else
                throw new ArgumentException($"MediaPlayer needs to be of type {nameof(IMediaPlayer<MediaPlayer, VideoView>)} to use this extension", nameof(mediaPlayer));
        }
    }
}
