using System;
using MediaManager.Media;

namespace MediaManager.Abstractions.EventArguments
{
    public class MediaItemFailedEventArgs : EventArgs
    {
        public MediaItemFailedEventArgs(Exception ex, IMediaItem file)
        {
            this.MediaExeption = ex;
            File = file;
        }

        public Exception MediaExeption { get; set; }
        public IMediaItem File { get; set; }
    }
}
