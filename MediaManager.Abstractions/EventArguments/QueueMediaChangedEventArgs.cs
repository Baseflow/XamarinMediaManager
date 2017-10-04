using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class QueueMediaChangedEventArgs : EventArgs
    {
        public QueueMediaChangedEventArgs(IMediaItem item)
        {
            Item = item;
        }

        public IMediaItem Item { get; set; }
    }
}
