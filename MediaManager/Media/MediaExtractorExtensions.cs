using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MediaManager.Media
{
    public static class MediaExtractorExtensions
    {
        public static async Task<IMediaItem> FetchMediaItemMetaData(this IMediaItem mediaItem)
        {
            if (mediaItem.IsMetadataExtracted)
                return mediaItem;

            return mediaItem = await CrossMediaManager.Current.MediaExtractor.CreateMediaItem(mediaItem);
        }

        public static async Task<IEnumerable<IMediaItem>> FetchMediaQueueMetaData(this IMediaQueue mediaQueue)
        {
            var mediaItems = new List<IMediaItem>();
            foreach (var item in mediaQueue)
            {
                mediaItems.Add(await item.FetchMediaItemMetaData().ConfigureAwait(false));
            }
            return mediaItems;
        }
    }
}
