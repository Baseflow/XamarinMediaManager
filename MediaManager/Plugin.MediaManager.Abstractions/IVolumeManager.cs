using System;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager.Abstractions
{
    public delegate void VolumeChangedEventHandler(object sender, VolumeChangedEventArgs e);

    /// <summary>
    /// Manages the volume of media
    /// </summary>
    public interface IVolumeManager
    {
        /// <summary>
        /// Raised when the volume changes
        /// </summary>
        event VolumeChangedEventHandler VolumeChanged;

        /// <summary>
        /// The volume for the current MediaPlayer
        /// </summary>
        float CurrentVolume { get; set; }

        /// <summary>
        /// The Maximum volume that can be used
        /// </summary>
        float MaxVolume { get; set; }

        /// <summary>
        /// True if the sound is Muted
        /// </summary>
        bool Mute { get; set; }
    }
}
