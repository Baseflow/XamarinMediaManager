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
                /*var layer = AVPlayerLayer.FromPlayer(videoPlayer.Player);
                layer.Frame = videoView.Frame;
                videoView.PlayerLayer = layer;*/
                videoView.Player = videoPlayer.Player;
                videoPlayer.PlayerView = videoView;
            }
            else
                throw new ArgumentException($"MediaPlayer needs to be of type {nameof(IMediaPlayer<AVPlayer, VideoView>)} to use this extension", nameof(mediaPlayer));
        }
    }
}
