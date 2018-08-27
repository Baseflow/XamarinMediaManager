using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager
{
    public static class MediaManagerExtensions
    {
        public static Task Play(this IMediaManager mediaManager, IEnumerable<IMediaItem> items)
        {
            mediaManager.MediaQueue.Clear();
            foreach (var item in items)
            {
                mediaManager.MediaQueue.Add(item);
            }

            return mediaManager.Play(items.First());
        }

        public static Task PlayPreviousOrSeekToStart(this IMediaManager mediaManager)
        {
            if (mediaManager.Position < TimeSpan.FromSeconds(3))
                return mediaManager.PlayPrevious();
            else
                return mediaManager.SeekTo(TimeSpan.Zero);
        }

        public static bool IsPlaying(this IMediaManager mediaManager)
        {
            return mediaManager.Status == MediaPlayerStatus.Playing;
        }

        public static Task PlayPause(this IMediaManager mediaManager)
        {
            var status = mediaManager.Status;

            if (status == MediaPlayerStatus.Paused || status == MediaPlayerStatus.Stopped)
                return mediaManager.Play();
            else
                return mediaManager.Pause();
        }
    }
}
