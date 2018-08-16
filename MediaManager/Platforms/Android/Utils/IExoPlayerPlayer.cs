using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager.Platforms.Android.Utils
{
    internal interface IExoPlayerPlayer
    {
        Com.Google.Android.Exoplayer2.SimpleExoPlayer Player { get; }
    }
}
