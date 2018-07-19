using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager
{
    public static class MediaManagerExtensions
    {
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
    }
}
