using System;
using AVFoundation;
using MediaManager.Platforms.Tvos.Video;

namespace MediaManager
{
    public static partial class MediaPlayerExtensions
    {
        public static void SetPlayerView(this IMediaPlayer mediaPlayer, VideoSurface videoView)
        {
            if (mediaPlayer is IMediaPlayer<AVPlayer, VideoSurface> videoPlayer)
            {
                var layer = AVPlayerLayer.FromPlayer(videoPlayer.Player);
                layer.Frame = videoView.Frame;
                layer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                videoView.Layer.AddSublayer(layer);
                videoPlayer.PlayerView = videoView;
            }
            else
                throw new ArgumentException($"MediaPlayer needs to be of type {nameof(IMediaPlayer<AVPlayer, VideoSurface>)} to use this extension", nameof(mediaPlayer));
        }
    }
}
