using Foundation;
using MediaManager.Library;
using MediaManager.Media;

namespace MediaManager.Platforms.Apple.Media
{
    public static class MediaItemExtensions
    {
        public static NSUrl GetNSUrl(this IMediaItem mediaItem)
        {
            var isLocallyAvailable = mediaItem.MediaLocation.IsLocal();
            return isLocallyAvailable ? new NSUrl(mediaItem.MediaUri, false) : new NSUrl(mediaItem.MediaUri);
        }
    }
}
