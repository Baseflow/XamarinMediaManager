using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class MediaFileFailedEventArgs : EventArgs
    {
        public MediaFileFailedEventArgs(Exception ex, IMediaItem item)
        {
            this.MediaExeption = ex;
            Item = item;
        }

        public Exception MediaExeption { get; set; }
        public IMediaItem Item { get; set; }
    }
}