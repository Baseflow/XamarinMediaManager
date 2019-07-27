using MediaManager.Volume;

namespace MediaManager.Platforms.Uap.Volume
{
    public class VolumeManager : IVolumeManager
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Windows;

        public int CurrentVolume { get; set; }
        public int MaxVolume { get; set; }
        public bool Muted { get; set; }

        public float Balance { get; set; }

        public event VolumeChangedEventHandler VolumeChanged;
    }
}
