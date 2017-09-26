using System;

namespace MediaManager.Abstractions.EventArguments
{
    public class MediaFinishedEventArgs : EventArgs
    {
        public MediaFinishedEventArgs(IMediaItem file)
        {
            File = file;
        }

        public IMediaItem File { get; set; }
    }
}
