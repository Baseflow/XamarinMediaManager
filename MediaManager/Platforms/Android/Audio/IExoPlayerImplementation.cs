using System;
using System.Collections.Generic;
using System.Text;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;

namespace MediaManager.Platforms.Android.Audio
{
    public interface IExoPlayerImplementation
    {
        SimpleExoPlayer Player { get; }
        MediaSessionCompat MediaSession { get; set; }
    }
}
