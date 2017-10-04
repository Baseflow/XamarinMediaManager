using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class MediaFinishedEventArgs : EventArgs
    {
        public MediaFinishedEventArgs(IMediaItem item)
        {
            Item = item;
        }

        public IMediaItem Item { get; set; }
    }
}
