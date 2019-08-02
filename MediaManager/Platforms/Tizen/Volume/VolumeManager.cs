using MediaManager.Volume;

namespace MediaManager.Platforms.Tizen.Volume
{
    public class VolumeManager : VolumeManagerBase, IVolumeManager
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Tizen;

        public override int CurrentVolume { get; set; }
        public override int MaxVolume { get; set; }
        public override float Balance { get; set; }
        public override bool Muted { get; set; }

        public override event VolumeChangedEventHandler VolumeChanged;
    }
}
