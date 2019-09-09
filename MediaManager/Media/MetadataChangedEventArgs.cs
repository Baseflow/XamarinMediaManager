using MediaManager.Library;

namespace MediaManager.Media
{
    public class MetadataChangedEventArgs : MediaItemEventArgs
    {
        public MetadataChangedEventArgs(IMediaItem mediaItem) : base(mediaItem)
        {
        }
    }
}
