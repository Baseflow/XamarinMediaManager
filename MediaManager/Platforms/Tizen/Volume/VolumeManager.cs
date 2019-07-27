using MediaManager.Volume;

namespace MediaManager.Platforms.Tizen.Volume
{
    public class VolumeManager : IVolumeManager
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Tizen;

        public int CurrentVolume { get; set; }
        public int MaxVolume { get; set; }
        public bool Muted { get; set; }

        public float Balance { get; set; }

        public event VolumeChangedEventHandler VolumeChanged;
    }
}
