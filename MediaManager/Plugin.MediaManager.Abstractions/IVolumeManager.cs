using System;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager.Abstractions
{
    public delegate void VolumeChangedEventHandler(object sender, VolumeChangedEventArgs e);

    public interface IVolumeManager
    {
        /// <summary>
        /// Raised when media is finished playing.
        /// </summary>
        event VolumeChangedEventHandler VolumeChanged;

        float CurrentVolume { get; set; }

        float MaxVolume { get; set; }

        bool Mute { get; set; }
    }
}
