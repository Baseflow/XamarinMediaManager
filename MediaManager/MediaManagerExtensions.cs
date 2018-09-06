using System;
using System.Threading.Tasks;
using MediaManager.Abstractions.Enums;

namespace MediaManager
{
    public static class MediaManagerExtensions
    {
        public static Task PlayPreviousOrSeekToStart(this IMediaManager mediaManager)
        {
            if (mediaManager.Position < TimeSpan.FromSeconds(3))
                return mediaManager.PlayPrevious();
            else
                return mediaManager.SeekTo(TimeSpan.Zero);
        }

        public static bool IsPlaying(this IMediaManager mediaManager)
        {
            return mediaManager.State == MediaPlayerState.Playing;
        }

        public static Task PlayPause(this IMediaManager mediaManager)
        {
            var status = mediaManager.State;

            if (status == MediaPlayerState.Paused || status == MediaPlayerState.Stopped)
                return mediaManager.Play();
            else
                return mediaManager.Pause();
        }
    }
}
