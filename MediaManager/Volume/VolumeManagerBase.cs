namespace MediaManager.Volume
{
    public abstract class VolumeManagerBase : NotifyPropertyChangedBase, IVolumeManager
    {
        public abstract int CurrentVolume { get; set; }
        public abstract int MaxVolume { get; set; }
        public abstract float Balance { get; set; }
        public abstract bool Muted { get; set; }

        public abstract event VolumeChangedEventHandler VolumeChanged;
    }
}
