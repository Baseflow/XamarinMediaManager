using System;
using MediaManager.Abstractions.Enums;

namespace MediaManager.Abstractions.EventArguments
{
    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs(MediaPlayerState status)
        {
            Status = status;
        }

        public MediaPlayerState Status { get; }
    }
}
