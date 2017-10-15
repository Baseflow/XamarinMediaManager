using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.V4.Media;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager
{
    public class VolumeManagerImplementation : VolumeProviderCompat.Callback, IVolumeManager
    {
        private AudioManager _audioManager = null;

        public VolumeManagerImplementation()
        {
            _audioManager = (AudioManager) Application.Context.GetSystemService(Context.AudioService);
        }

        public int CurrentVolume
        {
            get => _audioManager.GetStreamVolume(Stream.Music);
            set
            {
                int.TryParse(value.ToString(), out var vol);
                if (vol > MaxVolume) vol = MaxVolume;
                if (vol < 0) vol = 0;
                _audioManager.SetStreamVolume(Stream.Music, vol, VolumeNotificationFlags.RemoveSoundAndVibrate);
            }
        }

        public int MaxVolume {
            get { return _audioManager.GetStreamMaxVolume(Stream.Music); }
            set { }
        }

        public event VolumeChangedEventHandler VolumeChanged;

        public override void OnVolumeChanged(VolumeProviderCompat volumeProvider)
        {
            VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(volumeProvider.CurrentVolume, Muted));
        }

        //private bool _Mute;
        //public bool Mute
        //{
        //    get { return _Mute; }
        //    set
        //    {
        //        if (_Mute == value)
        //            return;
        //        _Mute = value;
        //    }
        //}

        private bool muted;
        public bool Muted
        {
            get { return muted; }
            set
            {
                if (muted == value)
                    return;
                Adjust flag = Adjust.Unmute;
                if (value)
                    flag = Adjust.Mute;
                _audioManager.AdjustStreamVolume(Stream.Music, flag, 0);
                _audioManager.AdjustVolume(flag, 0);
                muted = value;
            }
        }
    }
}
