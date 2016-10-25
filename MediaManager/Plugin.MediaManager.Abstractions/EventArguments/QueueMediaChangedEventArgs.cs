using System;
namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class QueueMediaChangedEventArgs : EventArgs
    {
        public QueueMediaChangedEventArgs(IMediaFile file)
        {
            File = file;
        }

        public IMediaFile File { get; set; }
    }
}
