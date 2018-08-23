using Android.Support.V4.Media;
using MediaManager.Media;

namespace MediaManager
{
    public static class AndroidMediaQueueExtensions
    {
        internal static MediaDescriptionCompat GetDescriptionCompat(this IMediaItem item)
        {
            var description = new MediaDescriptionCompat.Builder()
                .SetMediaId("test")
                .SetMediaUri(Android.Net.Uri.Parse(item.MetadataMediaUri))
                .SetTitle("Title")
                .SetSubtitle("Subtitle (artist?)")
                .SetDescription("Description")
                .SetExtras(null)
                .SetIconBitmap(null)
                .SetIconUri(null)
                .Build();

            return description; //new MediaSessionCompat.QueueItem(description, BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0));
        }
    }
}
