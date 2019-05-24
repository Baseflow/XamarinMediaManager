using System;
using System.Collections.Generic;
using System.Text;
using MediaManager.Platforms.Uap.Video;
using Windows.Media.Playback;

namespace MediaManager
{
    public static partial class MediaPlayerExtensions
    {
        public static void SetPlayerView(this IMediaPlayer mediaPlayer, VideoView videoView)
        {
            if (mediaPlayer is IMediaPlayer<MediaPlayer, VideoView> videoPlayer)
            {
                videoView.PlayerView.SetMediaPlayer(videoPlayer.Player);
                videoPlayer.PlayerView = videoView;
            }
            else
                throw new ArgumentException($"MediaPlayer needs to be of type {nameof(IMediaPlayer<MediaPlayer, VideoView>)} to use this extension", nameof(mediaPlayer));
        }
    }
}
