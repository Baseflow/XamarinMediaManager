using Windows.Media.Playback;
using MediaPlayerState = MediaManager.Player.MediaPlayerState;

namespace MediaManager.Platforms.Uap.Player
{
    public static class PlaybackStateExtensions
    {
        public static MediaPlayerState ToMediaPlayerState(this MediaPlaybackState playbackState)
        {
            switch (playbackState)
            {
                case MediaPlaybackState.None:
                    return MediaPlayerState.Stopped;
                case MediaPlaybackState.Opening:
                    return MediaPlayerState.Loading;
                case MediaPlaybackState.Buffering:
                    return MediaPlayerState.Buffering;
                case MediaPlaybackState.Playing:
                    return MediaPlayerState.Playing;
                case MediaPlaybackState.Paused:
                    return MediaPlayerState.Paused;
                default:
                    return MediaPlayerState.Stopped;
            }
        }
    }
}
