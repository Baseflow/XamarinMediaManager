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
        /// Valid values are 0 - MaxVolume
        /// </summary>
        int CurrentVolume { get; set; }

        /// <summary>
        /// The Maximum volume that can be used
        /// </summary>
        int MaxVolume { get; set; }

        /// <summary>
        /// True if the sound is Muted
        /// </summary>
        bool Muted { get; set; }
    }
}
