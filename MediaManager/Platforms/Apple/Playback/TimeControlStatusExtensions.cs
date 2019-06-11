using System;
using System.Collections.Generic;
using System.Text;
using AVFoundation;
using MediaManager.Playback;

namespace MediaManager.Platforms.Apple.Playback
{
    public static class TimeControlStatusExtensions
    {
        public static MediaPlayerState ToMediaPlayerState(this AVPlayerTimeControlStatus timeControlStatus)
        {
            switch (timeControlStatus)
            {
                case AVPlayerTimeControlStatus.Paused:
                    return MediaPlayerState.Paused;
                case AVPlayerTimeControlStatus.WaitingToPlayAtSpecifiedRate:
                    return MediaPlayerState.Buffering;
                case AVPlayerTimeControlStatus.Playing:
                    return MediaPlayerState.Playing;
            }
            return MediaPlayerState.Stopped;
        }
    }
}
