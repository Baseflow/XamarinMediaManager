using MediaManager.Media;

namespace MediaManager.Queue
{
    public class QueueChangedEventArgs : MediaItemEventArgs
    {
        public QueueChangedEventArgs(Library.IMediaItem mediaItem) : base(mediaItem)
        {
        }
    }
}
