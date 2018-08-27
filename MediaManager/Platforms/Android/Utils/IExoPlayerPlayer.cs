using System;
using System.Collections.Generic;
using System.Text;
using Com.Google.Android.Exoplayer2;

namespace MediaManager.Platforms.Android.Utils
{
    public interface IExoPlayerPlayer
    {
        IExoPlayer Player { get; }
    }
}
