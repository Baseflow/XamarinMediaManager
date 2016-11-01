using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class MediaFileFailedEventArgs : EventArgs
    {
        public MediaFileFailedEventArgs(Exception ex, IMediaFile file)
        {
            this.MediaExeption = ex;
            File = file;
        }

        public Exception MediaExeption { get; set; }
        public IMediaFile File { get; set; }
    }
}