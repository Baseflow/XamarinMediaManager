using System;
using MediaManager.Abstractions.Enums;

namespace MediaManager.Abstractions.EventArguments
{
    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs(PlaybackState status)
        {
            Status = status;
        }
        public PlaybackState Status { get; }
    }
}