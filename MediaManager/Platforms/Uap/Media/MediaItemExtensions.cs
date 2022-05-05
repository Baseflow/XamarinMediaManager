using System;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Library;
using MediaManager.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace MediaManager.Platforms.Uap.Media
{
    public static class MediaItemExtensions
    {
        public static async Task<MediaSource> ToMediaSource(this IMediaItem mediaItem)
        {
            //TODO: Get Metadata from MediaSource
            if (mediaItem.MediaLocation.IsLocal())
            {
                var storageFile = await StorageFile.GetFileFromPathAsync(mediaItem.MediaUri);
                return MediaSource.CreateFromStorageFile(storageFile);
            } 
            else if (mediaItem.MediaLocation == MediaLocation.InMemory)
                return MediaSource.CreateFromStream(mediaItem.Data.AsRandomAccessStream(), mediaItem.MimeType.ToMimeTypeString());
            else
                return MediaSource.CreateFromUri(new Uri(mediaItem.MediaUri));
        }

        public static MediaPlaybackItem ToMediaPlaybackItem(this MediaSource mediaSource)
        {
            return new MediaPlaybackItem(mediaSource);
        }

        public static MediaPlaybackItem ToMediaPlaybackItem(this MediaSource mediaSource, TimeSpan startAt, TimeSpan? stopAt = null)
        {
            if (stopAt is TimeSpan endTime)
                return new MediaPlaybackItem(mediaSource, startAt, endTime);

            return new MediaPlaybackItem(mediaSource, startAt);
        }
    }
}
