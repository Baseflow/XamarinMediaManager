using MediaManager.Volume;

namespace MediaManager.Platforms.Wpf.Volume
{
    public class VolumeManager : IVolumeManager
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Wpf;

        public int CurrentVolume { get; set; }
        public int MaxVolume { get; set; }
        public bool Muted {
            get => MediaManager.Player.IsMuted;
            set => MediaManager.Player.IsMuted = value;
        }

        public float Balance { get; set; }

        public event VolumeChangedEventHandler VolumeChanged;
    }
}
