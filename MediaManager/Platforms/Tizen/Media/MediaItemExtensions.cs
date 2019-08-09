using MediaManager.Library;
using Tizen.Multimedia;

namespace MediaManager.Platforms.Tizen.Media
{
    public static class MediaItemExtensions
    {
        private static MediaManagerImplementation MediaManager => CrossMediaManager.Tizen;

        public static MediaSource ToMediaSource(this IMediaItem mediaItem)
        {
            switch (mediaItem.MediaLocation)
            {
                case MediaLocation.Remote:
                    return new MediaUriSource(mediaItem.MediaUri);
                case MediaLocation.FileSystem:
                case MediaLocation.Embedded:
                case MediaLocation.Unknown:
                default:
                    return new MediaUriSource(mediaItem.MediaUri);
            }

        }
    }
}
