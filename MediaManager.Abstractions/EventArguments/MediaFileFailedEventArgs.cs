using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class MediaItemFailedEventArgs : EventArgs
    {
        public MediaItemFailedEventArgs(Exception ex, IMediaItem item)
        {
            this.MediaExeption = ex;
            Item = item;
        }

        public Exception MediaExeption { get; set; }
        public IMediaItem Item { get; set; }
    }
}