using Foundation;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager
{
    public static class MediaFileUrlHelper
    {
        public static NSUrl GetUrlFor(IMediaFile mediaFile) {
            var isFile = mediaFile.Type == MediaFileType.AudioFile || mediaFile.Type == MediaFileType.VideoFile;

            NSUrl url;

            if (isFile)
            {
                url = new NSUrl(mediaFile.Url, false);
            }
            else
            {
                url = new NSUrl(mediaFile.Url);
            }

            return url;
        }
    }
}
