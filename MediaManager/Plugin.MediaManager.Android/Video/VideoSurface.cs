using System;
using Android.Content;
using Android.Widget;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    public class VideoSurface : VideoView, IVideoSurface
    {
        public VideoSurface(Context context) : base(context)
        {

        }
    }
}
