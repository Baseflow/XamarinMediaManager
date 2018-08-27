using Android.Graphics;
using Android.Support.V4.Media;
using MediaManager.Media;

namespace MediaManager
{
    public static class MediaQueueExtensions
    {
        public static MediaDescriptionCompat GetMediaDescription(this IMediaItem item)
        {
            var description = new MediaDescriptionCompat.Builder()
                .SetMediaId(item?.MetadataMediaUri)
                .SetMediaUri(Android.Net.Uri.Parse(item?.MetadataMediaUri))
                .SetTitle(item?.MetadataTitle)
                .SetSubtitle(item?.MetadataArtist)
                .SetDescription(item?.MetadataDisplayDescription)
                .SetExtras(null)
                .SetIconBitmap(item?.MetadataAlbumArt as Bitmap)
                .SetIconUri(item?.MetadataDisplayIconUri != null ? Android.Net.Uri.Parse(item?.MetadataDisplayIconUri) : null)
                .Build();

            return description;
        }

        public static MediaBrowserCompat.MediaItem GetMediaItem(this IMediaItem item)
        {
            var media = new MediaBrowserCompat.MediaItem(GetMediaDescription(item), MediaBrowserCompat.MediaItem.FlagPlayable);
            return media;
        }

        public static IMediaItem ToMediaItem(this MediaDescriptionCompat mediaDescription)
        {
            var item = new MediaItem();
            item.MetadataMediaUri = mediaDescription.MediaUri.ToString();
            return item;
        }
    }
}
