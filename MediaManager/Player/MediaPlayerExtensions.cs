using System;
using MediaManager.Video;

namespace MediaManager
{
    public static partial class MediaPlayerExtensions
    {
        public static IVideoView GetPlayerView(this IMediaPlayer mediaPlayer)
        {
            //TODO: IVideoView should be IVideoPlayer
            if (mediaPlayer is IVideoView videoPlayer)
                return videoPlayer;
            else
                throw new ArgumentException("MediaPlayer needs to be of type IVideoView to use this extension", nameof(mediaPlayer));
        }
    }
}
