using MediaManager.Library;
using MediaManager.Media;

namespace MediaManager.Queue
{
    public class QueueEndedEventArgs : MediaItemEventArgs
    {
        public QueueEndedEventArgs(IMediaItem mediaItem) : base(mediaItem)
        {
        }
    }
}
