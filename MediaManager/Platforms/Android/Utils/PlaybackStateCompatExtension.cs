using System;
using System.Collections.Generic;
using System.Text;
using Android.OS;
using Android.Support.V4.Media.Session;
using MediaManager.Media;

namespace MediaManager.Platforms.Android.Utils
{
    public partial class PlaybackStateCompatExtension
    {
        private readonly PlaybackStateCompat state;
        public PlaybackStateCompatExtension(PlaybackStateCompat state)
        {
            this.state = state;
        }

        public MediaPlayerStatus Status
        {
            get
            {
                switch (state.State)
                {
                    case PlaybackStateCompat.StateFastForwarding:
                    case PlaybackStateCompat.StateRewinding:
                    case PlaybackStateCompat.StateSkippingToNext:
                    case PlaybackStateCompat.StateSkippingToPrevious:
                    case PlaybackStateCompat.StateSkippingToQueueItem:
                    case PlaybackStateCompat.StatePlaying:
                        return MediaPlayerStatus.Playing;

                    case PlaybackStateCompat.StatePaused:
                        return MediaPlayerStatus.Paused;

                    case PlaybackStateCompat.StateConnecting:
                    case PlaybackStateCompat.StateBuffering:
                        return MediaPlayerStatus.Buffering;

                    case PlaybackStateCompat.StateError:
                    case PlaybackStateCompat.StateStopped:
                        return MediaPlayerStatus.Stopped;

                    default:
                        return MediaPlayerStatus.Stopped;
                }
            }
        }

        public TimeSpan Position
        {
            get
            {
                long currentPosition = state.Position;

                if (Status == MediaPlayerStatus.Playing)
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
        }

        public TimeSpan Buffered => TimeSpan.FromMilliseconds(state.BufferedPosition);
    }
}
