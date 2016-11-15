using System;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    public class VolumeManagerImplementation : IVolumeManager
    {
        public VolumeManagerImplementation()
        {
        }

        public int CurrentVolume
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int MaxVolume
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public event VolumeChangedEventHandler VolumeChanged;
    }
}
