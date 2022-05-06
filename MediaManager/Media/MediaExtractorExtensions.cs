using System.IO;
using MediaManager.Library;
using MediaManager.Queue;

namespace MediaManager.Media
{
    public static class MediaExtractorExtensions
    {
        private static IMediaExtractor MediaExtractor => CrossMediaManager.Current.Extractor;

        public static bool IsRemote(this MediaLocation mediaLocation)
        {
            return mediaLocation == MediaLocation.Remote;
        }

        public static bool IsLocal(this MediaLocation mediaLocation)
        {
            return (mediaLocation == MediaLocation.FileSystem) || (mediaLocation == MediaLocation.Embedded) || (mediaLocation == MediaLocation.Resource);
        }

        public static async Task<IEnumerable<IMediaItem>> CreateMediaItems(this IEnumerable<string> items)
        {
            var mediaItems = items.Select(i => MediaExtractor.CreateMediaItem(i));
            return await Task.WhenAll(mediaItems).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<IMediaItem>> CreateMediaItems(this IEnumerable<FileInfo> items)
        {
            var mediaItems = items.Select(i => MediaExtractor.CreateMediaItem(i));
            return await Task.WhenAll(mediaItems).ConfigureAwait(false);
        }

        public static async Task<IMediaItem> UpdateMediaItem(this IMediaItem mediaItem)
        {
            if (mediaItem.IsMetadataExtracted)
                return mediaItem;

            return await MediaExtractor.UpdateMediaItem(mediaItem).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<IMediaItem>> UpdateMediaItems(this IEnumerable<IMediaItem> items)
        {
            var mediaItems = items.Select(i => i.UpdateMediaItem());
            return await Task.WhenAll(mediaItems).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<IMediaItem>> UpdateMediaItems(this IMediaQueue mediaQueue)
        {
            var mediaItems = mediaQueue.Select(i => i.UpdateMediaItem());
            return await Task.WhenAll(mediaItems).ConfigureAwait(false);
        }
    }
}
