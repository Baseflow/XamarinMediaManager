using Android.Media;
using Android.Runtime;
using MediaManager.Platforms.Android.Media;
using System;

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
