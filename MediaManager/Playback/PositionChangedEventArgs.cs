namespace MediaManager.Playback
{
    public class PositionChangedEventArgs : EventArgs
    {
        public PositionChangedEventArgs(TimeSpan position)
        {
            Position = position;
        }

        public TimeSpan Position { get; }
    }
}
