using System;
using System.Collections.Generic;
using System.Text;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.UI;

namespace MediaManager
{
    public static class MediaPlayerExtensions
    {
        public static void SetPlayerView(this IMediaPlayer mediaPlayer, PlayerView playerView)
        {
            if (mediaPlayer is IMediaPlayer<SimpleExoPlayer> exoPlayer)
            {
                playerView.RequestFocus();
                playerView.Player = exoPlayer.Player;
            }
            else
                throw new ArgumentException("MediaPlayer needs to be of type IMediaPlayer<SimpleExoPlayer> to use this extension", nameof(mediaPlayer));
        }
    }
}
