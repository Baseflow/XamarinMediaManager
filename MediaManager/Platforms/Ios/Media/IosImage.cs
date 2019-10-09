using System;
using MediaManager.Media;

namespace MediaManager.Platforms.Ios.Media
{
    public class IosImage: UIImage, IImage<UIImage>
    {
        public IosImage()
        {
        }

        public UIImage ToNative()
        {
            return this;
        }
    }
}
