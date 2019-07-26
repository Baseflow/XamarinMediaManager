using AVFoundation;
using MediaManager.Player;

namespace MediaManager.Platforms.Apple.Playback
{
    public static class MediaPlayerStateExtensions
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

        public static MediaPlayerState ToMediaPlayerState(this AVPlayerStatus status)
        {
            switch (status)
            {
                case AVPlayerStatus.Unknown:
                    return MediaPlayerState.Stopped;
                case AVPlayerStatus.ReadyToPlay:
                    return MediaPlayerState.Paused;
                case AVPlayerStatus.Failed:
                    return MediaPlayerState.Failed;
            }
            return MediaPlayerState.Stopped;
        }
    }
}
