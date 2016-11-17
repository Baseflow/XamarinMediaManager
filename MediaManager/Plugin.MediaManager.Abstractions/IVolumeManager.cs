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

        int CurrentVolume { get; set; }

        int MaxVolume { get; set; }

        bool Mute { get; set; }
    }
}
