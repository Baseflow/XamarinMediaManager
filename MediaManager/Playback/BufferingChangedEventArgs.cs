using System;

namespace MediaManager.Playback
{
    public class BufferingChangedEventArgs : EventArgs
    {
        public BufferingChangedEventArgs(TimeSpan buffered)
        {
            Buffered = buffered;
        }

        public TimeSpan Buffered { get; }
    }
}
