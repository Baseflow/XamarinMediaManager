using System;

namespace MediaManager.Abstractions.EventArguments
{
    public class BufferingChangedEventArgs : EventArgs
    {
        public BufferingChangedEventArgs(double bufferProgress, TimeSpan bufferedTime)
        {
            BufferProgress = bufferProgress;
            BufferedTime = bufferedTime;
        }

        public double BufferProgress { get; }
        public TimeSpan BufferedTime { get; }
    }
}
