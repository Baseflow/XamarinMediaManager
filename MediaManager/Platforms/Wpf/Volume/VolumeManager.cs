using MediaManager.Volume;

namespace MediaManager.Platforms.Wpf.Volume
{
    public class VolumeManager : VolumeManagerBase, IVolumeManager
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Wpf;

        public override int CurrentVolume { get; set; }

        public override int MaxVolume { get; set; }

        public override bool Muted
        {
            get => MediaManager.Player.IsMuted;
            set => MediaManager.Player.IsMuted = value;
        }

        public override float Balance { get; set; }

        public override event VolumeChangedEventHandler VolumeChanged;
    }
}
