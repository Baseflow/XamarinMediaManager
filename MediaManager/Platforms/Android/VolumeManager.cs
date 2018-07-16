using MediaManager.Volume;

namespace MediaManager
{
    public class VolumeManager : IVolumeManager
    {
        public int CurrentVolume { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int MaxVolume { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool Muted { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public event VolumeChangedEventHandler VolumeChanged;
    }
}
