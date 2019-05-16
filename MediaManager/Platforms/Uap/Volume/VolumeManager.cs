using System;
using System.Collections.Generic;
using System.Text;
using MediaManager.Volume;

namespace MediaManager.Platforms.Uap.Volume
{
    public class VolumeManager : IVolumeManager
    {
        public int CurrentVolume { get; set; }
        public int MaxVolume { get; set; }
        public bool Muted { get; set; }

        public event VolumeChangedEventHandler VolumeChanged;
    }
}
