using System;
using System.Collections.Generic;
using System.Text;
using MediaManager.Volume;

namespace MediaManager.Platforms.Wpf.Volume
{
    public class VolumeManager : IVolumeManager
    {
        public int CurrentVolume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MaxVolume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Muted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event VolumeChangedEventHandler VolumeChanged;
    }
}
