using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;

namespace MediaManager.Platforms.Android
{
    public static class MediaManagerExtensions
    {
        public static void SetContext(this IMediaManager mediaManager, Context context)
        {
            (mediaManager as MediaManagerImplementation).Context = context;
        }
    }
}
