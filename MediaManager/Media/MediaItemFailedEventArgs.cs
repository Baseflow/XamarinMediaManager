using MediaManager.Library;

namespace MediaManager.Media
{
    public class MediaItemFailedEventArgs : MediaItemEventArgs
    {
        public MediaItemFailedEventArgs(IMediaItem mediaItem, Exception exception, string message) : base(mediaItem)
        {
            Exeption = exception;
            Message = message;
        }

        public Exception Exeption { get; protected set; }
        public string Message { get; protected set; }
    }
}
