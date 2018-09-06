using System;
using MediaManager.Media;

namespace MediaManager.Abstractions.EventArguments
{
    public class MediaFileChangedEventArgs : EventArgs
    {
        public MediaFileChangedEventArgs(IMediaItem file)
        {
            File = file;
        }

        public IMediaItem File { get; set; }
    }
}
