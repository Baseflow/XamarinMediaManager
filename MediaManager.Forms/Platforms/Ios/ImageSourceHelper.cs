using CoreGraphics;
using UIKit;
using Xamarin.Forms;

namespace MediaManager.Forms.Platforms.Ios
{
    public static class ImageSourceHelper
    {
        public static ImageSource ToImageSource(this CGImage cgImage)
        {
            return ImageSource.FromStream(() => new UIImage(cgImage).AsPNG().AsStream());
        }

        public static ImageSource ToImageSource(this UIImage uIImage)
        {
            return ImageSource.FromStream(() => uIImage.AsPNG().AsStream());
        }
    }
}
