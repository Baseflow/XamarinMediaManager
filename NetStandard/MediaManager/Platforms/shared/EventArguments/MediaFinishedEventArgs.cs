using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class MediaFinishedEventArgs : EventArgs
    {
        public MediaFinishedEventArgs(IMediaFile file)
        {
            File = file;
        }

        public IMediaFile File { get; set; }
    }
}
