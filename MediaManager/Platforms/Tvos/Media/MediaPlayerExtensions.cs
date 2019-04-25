using System;
using AVFoundation;
using MediaManager.Platforms.Tvos.Video;
using MediaManager.Video;

namespace MediaManager
{
    public static partial class MediaPlayerExtensions
    {
        public static void SetPlayerView(this IMediaPlayer mediaPlayer, VideoSurface videoView)
        {
            if (mediaPlayer is IVideoPlayer<AVPlayer, VideoSurface> videoPlayer)
            {
                var layer = AVPlayerLayer.FromPlayer(videoPlayer.Player);
                layer.Frame = videoView.Frame;
                layer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                videoView.Layer.AddSublayer(layer);
                videoPlayer.PlayerView = videoView;
            }
            else
                throw new ArgumentException($"MediaPlayer needs to be of type {nameof(IVideoPlayer<AVPlayer, VideoSurface>)} to use this extension", nameof(mediaPlayer));
        }
    }
}
