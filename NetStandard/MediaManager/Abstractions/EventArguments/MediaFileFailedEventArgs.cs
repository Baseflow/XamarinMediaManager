using System;

namespace MediaManager.Abstractions.EventArguments
{
    public class MediaFileFailedEventArgs : EventArgs
    {
        public MediaFileFailedEventArgs(Exception ex, IMediaItem file)
        {
            this.MediaExeption = ex;
            File = file;
        }

        public Exception MediaExeption { get; set; }
        public IMediaItem File { get; set; }
    }
}