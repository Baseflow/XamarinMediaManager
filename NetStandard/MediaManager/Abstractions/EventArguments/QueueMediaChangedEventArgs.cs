using System;

namespace MediaManager.Abstractions.EventArguments
{
    public class QueueMediaChangedEventArgs : EventArgs
    {
        public QueueMediaChangedEventArgs(IMediaItem file)
        {
            File = file;
        }

        public IMediaItem File { get; set; }
    }
}
