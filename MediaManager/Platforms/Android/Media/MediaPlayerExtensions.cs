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
                playerView.Player = exoPlayer.Player;
            }
        }
    }
}
