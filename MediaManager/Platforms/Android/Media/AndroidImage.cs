using System;
using MediaManager.Media;
using Android.Graphics.Drawables;

namespace MediaManager.Platforms.Android.Media
{
    public class AndroidImage : IImage
    {
        public AndroidImage(Drawable drawable)
        {

        }

        public Drawable ToNative<Drawable>()
        {
            return this;
        }
    }
}
