using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager
{
    public static class MediaManagerExtensions
    {
        public static Task Play(this IMediaManager mediaManager, IMediaItem item)
        {
            return mediaManager.PlaybackManager.CurrentMediaPlayer.Play(item);
        }

        public static Task Play(this IMediaManager mediaManager, IEnumerable<IMediaItem> items)
        {
            mediaManager.MediaQueue.Add(items);
            return mediaManager.PlaybackManager.CurrentMediaPlayer.Play(item);
        }
    }
}
