using Android.Support.V4.Media.Session;
using MediaManager.Player;

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
                case PlaybackStateCompat.StateStopped:
                    return MediaPlayerState.Stopped;

                case PlaybackStateCompat.StateError:
                    return MediaPlayerState.Failed;

                default:
                    return MediaPlayerState.Stopped;
            }
        }
    }
}
