using System;

namespace Plugin.MediaManager.Abstractions
{
    public class PlayerStatusChangedEventArgs : EventArgs
    {
        public PlayerStatusChangedEventArgs(PlayerStatus status)
        {
            Status = status;
        }
        public PlayerStatus Status { get; }
    }
}