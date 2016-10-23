using System;

namespace Plugin.MediaManager.Abstractions
{
    public class PlaybackPositionChangedEventArgs : EventArgs
    {
        public PlaybackPositionChangedEventArgs(double progress, TimeSpan position)
        {
            Progress = progress;
            Position = position;
        }
        public double Progress { get; }
        public TimeSpan Position { get; }
    }
}