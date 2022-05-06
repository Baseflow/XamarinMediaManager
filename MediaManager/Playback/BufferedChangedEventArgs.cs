namespace MediaManager.Playback
{
    public class BufferedChangedEventArgs : EventArgs
    {
        public BufferedChangedEventArgs(TimeSpan buffered)
        {
            Buffered = buffered;
        }

        public TimeSpan Buffered { get; }
    }
}
