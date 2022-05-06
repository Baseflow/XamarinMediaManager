namespace MediaManager.Volume
{
    public class VolumeChangedEventArgs : EventArgs
    {
        public VolumeChangedEventArgs(int newVolume, bool muted)
        {
            NewVolume = newVolume;
            Muted = muted;
        }

        public int NewVolume { get; }
        public bool Muted { get; }
    }
}
