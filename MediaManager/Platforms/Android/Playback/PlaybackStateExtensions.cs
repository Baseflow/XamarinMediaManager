using Android.Support.V4.Media.Session;
using MediaManager.Playback;

namespace MediaManager.Platforms.Android.Playback
{
    public static class PlaybackStateExtensions
    {
        public static MediaPlayerState ToMediaPlayerState(this PlaybackStateCompat playbackState)
        {
            return ToMediaPlayerState(playbackState?.State ?? PlaybackStateCompat.StateStopped);
        }

        public static MediaPlayerState ToMediaPlayerState(this int playbackState)
        {
            switch (playbackState)
            {
                case PlaybackStateCompat.StateFastForwarding:
                case PlaybackStateCompat.StateRewinding:
                case PlaybackStateCompat.StateSkippingToNext:
                case PlaybackStateCompat.StateSkippingToPrevious:
                case PlaybackStateCompat.StateSkippingToQueueItem:
                case PlaybackStateCompat.StatePlaying:
                    return MediaPlayerState.Playing;

                case PlaybackStateCompat.StatePaused:
                    return MediaPlayerState.Paused;

                case PlaybackStateCompat.StateConnecting:
                case PlaybackStateCompat.StateBuffering:
                    return MediaPlayerState.Buffering;

                case PlaybackStateCompat.StateNone:
                    return MediaPlayerState.Loading;

                case PlaybackStateCompat.StateError:
                case PlaybackStateCompat.StateStopped:
                    return MediaPlayerState.Stopped;
                
                default:
                    return MediaPlayerState.Stopped;
            }
        }
    }
}
