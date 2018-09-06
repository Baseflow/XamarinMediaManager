using System;
using MediaManager.Media;

namespace MediaManager.Abstractions.EventArguments
{
    public class QueueMediaChangedEventArgs : EventArgs
    {
        public QueueMediaChangedEventArgs(IMediaItem file)
        {
            Item = file;
        }

        public IMediaItem Item { get; set; }
    }
}
