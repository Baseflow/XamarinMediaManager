using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager
{
    public static class MediaManagerExtensions
    {
        public static Task Play(this IMediaManager mediaManager)
        {
            return mediaManager.PlaybackManager.Play();
        }

        public static Task Play(this IMediaManager mediaManager, IMediaItem item)
        {
            return mediaManager.PlaybackManager.Play(item);
        }

        public static Task Play(this IMediaManager mediaManager, IEnumerable<IMediaItem> items)
        {
            foreach (var item in items)
            {
                mediaManager.MediaQueue.Add(item);
            }

            return mediaManager.PlaybackManager.Play(items.First());
        }

        public static bool IsPlaying(this IMediaManager mediaManager)
        {
            return mediaManager.PlaybackManager.Status == MediaPlayerStatus.Playing;
        }

        public static Task Pause(this IMediaManager mediaManager)
        {
            return mediaManager.PlaybackManager.Pause();
        }

        public static Task PlayPause(this IMediaManager mediaManager)
        {
            var status = mediaManager.PlaybackManager.Status;

            if (status == MediaPlayerStatus.Paused || status == MediaPlayerStatus.Stopped)
                return mediaManager.Play();
            else
                return mediaManager.Pause();
        }

        public static Task PlayNext(this IMediaManager mediaManager)
        {
            return mediaManager.PlaybackManager.PlayNext();
        }

        public static Task PlayPrevious(this IMediaManager mediaManager)
        {
            return mediaManager.PlaybackManager.PlayPrevious();
        }

        public static Task SeekTo(this IMediaManager mediaManager, int totalMilliSeconds)
        {
            return mediaManager.PlaybackManager.SeekTo(TimeSpan.FromMilliseconds(totalMilliSeconds));
        }
    }
}
