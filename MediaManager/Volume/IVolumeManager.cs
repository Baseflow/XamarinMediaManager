using System.ComponentModel;

namespace MediaManager.Volume
{
    public delegate void VolumeChangedEventHandler(object sender, VolumeChangedEventArgs e);

    public interface IVolumeManager : INotifyPropertyChanged
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
        /// -1.0f (Left), 0.0f (Center), 1.0f (right)
        /// </summary>
        float Balance { get; set; }

        /// <summary>
        /// True if the sound is Muted
        /// </summary>
        bool Muted { get; set; }
    }
}
