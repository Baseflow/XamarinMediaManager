using System;
using System.Collections.Generic;
using System.Text;
using MediaManager.Playback;
using Windows.Media.Playback;

namespace MediaManager.Platforms.Uap.Player
{
    public static class PlaybackStateExtensions
    {
        public static Playback.MediaPlayerState ToMediaPlayerState(this MediaPlaybackState playbackState)
        {
            switch (playbackState)
            {
                case MediaPlaybackState.None:
                    return Playback.MediaPlayerState.Stopped;
                case MediaPlaybackState.Opening:
                    return Playback.MediaPlayerState.Loading;
                case MediaPlaybackState.Buffering:
                    return Playback.MediaPlayerState.Buffering;
                case MediaPlaybackState.Playing:
                    return Playback.MediaPlayerState.Playing;
                case MediaPlaybackState.Paused:
                    return Playback.MediaPlayerState.Paused;
                default:
                    return Playback.MediaPlayerState.Stopped;
            }
        }
    }
}
