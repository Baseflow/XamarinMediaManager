using System;
using System.Collections.Generic;
using System.Text;
using AVFoundation;
using MediaManager.Platforms.Ios.Video;
using MediaManager.Video;
using UIKit;

namespace MediaManager
{
    public static partial class MediaPlayerExtensions
    {
        public static void SetPlayerView(this IMediaPlayer mediaPlayer, VideoView videoView)
        {
            if (mediaPlayer is IVideoPlayer<AVPlayer, VideoView> videoPlayer)
            {
                var layer = AVPlayerLayer.FromPlayer(videoPlayer.Player);
                layer.Frame = videoView.Frame;
                layer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                videoView.Layer.AddSublayer(layer);
                videoPlayer.PlayerView = videoView;
            }
            else
                throw new ArgumentException($"MediaPlayer needs to be of type {nameof(IVideoPlayer<AVPlayer, VideoView>)} to use this extension", nameof(mediaPlayer));
        }
    }
}
