using Xamarin.Forms;

namespace MediaManager.Forms
{
    public static class ImageSourceHelper
    {
        public static ImageSource ToImageSource(this object image)
        {
#if ANDROID
            if (image is Android.Graphics.Bitmap bitmap)
                return MediaManager.Forms.Platforms.Android.ImageSourceHelper.ToImageSource(bitmap);
#elif IOS
            if (image is CoreGraphics.CGImage cgImage)
                return MediaManager.Forms.Platforms.Ios.ImageSourceHelper.ToImageSource(cgImage);
            else if (image is UIKit.UIImage uIImage)
                return MediaManager.Forms.Platforms.Ios.ImageSourceHelper.ToImageSource(uIImage);
#elif WINDOWS
            //TODO: This one should not be async. It might deadlock
            if (image is Windows.UI.Xaml.Media.Imaging.BitmapImage bitmapImage)
                return MediaManager.Forms.Platforms.Uap.ImageSourceHelper.ToImageSource(bitmapImage).GetAwaiter().GetResult();
#endif
            return null;
        }
    }
}
