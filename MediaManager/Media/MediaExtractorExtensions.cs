using System.Linq;
using System.Threading.Tasks;
using MediaManager.Queue;

namespace MediaManager.Media
{
    public static class MediaExtractorExtensions
    {
        public static async Task<IMediaItem> FetchMetaData(this IMediaItem mediaItem)
        {
            if (mediaItem.IsMetadataExtracted)
                return mediaItem;

            return mediaItem = await CrossMediaManager.Current.MediaExtractor.UpdateMediaItem(mediaItem).ConfigureAwait(false);
        }

        public static async Task<IMediaItem[]> FetchMetaData(this IMediaQueue mediaQueue)
        {
            var mediaItems = mediaQueue.Select(i => i.FetchMetaData());

            return await Task.WhenAll(mediaItems);
        }
    }
}
