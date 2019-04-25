using System;
using Android.Runtime;

namespace MediaManager.Platforms.Android.Audio
{
    public class AudioPlayer : Media.MediaPlayer
    {
        public AudioPlayer()
        {
        }

        public AudioPlayer(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }
    }
}
