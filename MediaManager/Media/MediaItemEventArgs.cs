using MediaManager.Library;

namespace MediaManager.Media
{
    public class MediaItemEventArgs : EventArgs
    {
        public MediaItemEventArgs(IMediaItem mediaItem)
        {
            MediaItem = mediaItem;
        }

        public IMediaItem MediaItem { get; protected set; }
    }
}
