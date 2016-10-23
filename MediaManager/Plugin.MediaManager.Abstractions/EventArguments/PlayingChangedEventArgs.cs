using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class PlayingChangedEventArgs : EventArgs
    {
        public PlayingChangedEventArgs(double progress, TimeSpan position)
        {
            Progress = progress;
            Position = position;
        }
        public double Progress { get; }
        public TimeSpan Position { get; }
    }
}