using Android.Support.V4.Media.Session;
using MediaManager.Playback;

namespace MediaManager.Platforms.Android.Playback
{
    public static class PlaybackStateExtensions
    {
        public static MediaPlayerState ToMediaPlayerState(this PlaybackStateCompat playbackState)
        {
            switch (playbackState.State)
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

                case PlaybackStateCompat.StateError:
                case PlaybackStateCompat.StateStopped:
                    return MediaPlayerState.Stopped;

                default:
                    return MediaPlayerState.Stopped;
            }
        }

        /*public TimeSpan Position
        {
            get
            {
                long currentPosition = state.Position;

                if (Status == MediaPlayerState.Playing)
                {
                    // Calculate the elapsed time between the last position update and now and unless
                    // paused, we can assume (delta * speed) + current position is approximately the
                    // latest position. This ensure that we do not repeatedly call the getPlaybackState()
                    // on MediaControllerCompat.
                    long timeDelta = SystemClock.ElapsedRealtime() - state.LastPositionUpdateTime;
                    currentPosition += (long)(timeDelta * state.PlaybackSpeed);
                }
                return TimeSpan.FromMilliseconds(currentPosition);
            }
        }*/
    }
}
