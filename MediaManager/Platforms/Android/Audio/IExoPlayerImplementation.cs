using System;
using System.Collections.Generic;
using System.Text;
using Com.Google.Android.Exoplayer2;

namespace MediaManager.Platforms.Android.Audio
{
    public interface IExoPlayerImplementation
    {
        SimpleExoPlayer Player { get; }
    }
}
