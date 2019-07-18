using System;
using System.Threading.Tasks;
using MediaManager.Media;
using Windows.Media.Core;
using Windows.Storage;

namespace MediaManager.Platforms.Uap.Media
{
    public static class MediaItemExtensions
    {
        public static async Task<MediaSource> ToMediaSource(this IMediaItem mediaItem)
        {
            //TODO: Get Metadata from MediaSource
            switch (mediaItem.MediaLocation)
            {
                case MediaLocation.FileSystem:
                    var storageFile = await StorageFile.GetFileFromPathAsync(mediaItem.MediaUri);
                    return MediaSource.CreateFromStorageFile(storageFile);
                default:
                    return MediaSource.CreateFromUri(new Uri(mediaItem.MediaUri));
            }
        }
    }
}
