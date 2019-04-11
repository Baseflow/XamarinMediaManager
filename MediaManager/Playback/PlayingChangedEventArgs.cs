using System;

namespace MediaManager.Playback
{
    public class PlayingChangedEventArgs : EventArgs
    {
        public PlayingChangedEventArgs(TimeSpan position, TimeSpan duration)
        {
            Position = position;
            Duration = duration;
        }
        public TimeSpan Position { get; }
        public TimeSpan Duration { get; }
    }
}
