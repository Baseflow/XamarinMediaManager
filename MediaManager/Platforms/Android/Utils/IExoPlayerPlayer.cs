using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager.Platforms.Android.Utils
{
    public interface IExoPlayerPlayer
    {
        Com.Google.Android.Exoplayer2.SimpleExoPlayer Player { get; }
    }
}
