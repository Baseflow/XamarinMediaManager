using Foundation;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager
{
    public static class MediaFileUrlHelper
    {
        public static NSUrl GetUrlFor(IMediaFile mediaFile) {
            var isLocallyAvailable = mediaFile.Availability == ResourceAvailability.Local;

            var url = isLocallyAvailable ? new NSUrl(mediaFile.Url, false) : new NSUrl(mediaFile.Url);

            return url;
        }
    }
}
