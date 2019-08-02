using System;
using MediaManager.Volume;

namespace MediaManager.Platforms.Uap.Volume
{
    public class VolumeManager : VolumeManagerBase, IVolumeManager
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Windows;

        public override event VolumeChangedEventHandler VolumeChanged;

        public VolumeManager()
        {
            MediaManager.Player.VolumeChanged += Player_VolumeChanged;
            MediaManager.Player.IsMutedChanged += Player_IsMutedChanged;
        }

        private void Player_IsMutedChanged(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            Muted = sender.IsMuted;
        }

        private void Player_VolumeChanged(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            CurrentVolume = Convert.ToInt32(sender.Volume);
        }

        public override int CurrentVolume { get; set; }
        public override int MaxVolume { get; set; }
        public override float Balance { get; set; }
        public override bool Muted { get; set; }
    }
}
