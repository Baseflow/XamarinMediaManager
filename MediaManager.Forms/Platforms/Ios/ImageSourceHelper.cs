using System.Threading.Tasks;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

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

        public static async Task<UIImage> ToNative(this ImageSource source)
        {
            var imageHandler = source.GetHandler();
            if (imageHandler == null)
                return null;

            var nativeImage = await imageHandler.LoadImageAsync(source);
            return nativeImage;
        }

        public static IImageSourceHandler GetHandler(this ImageSource source)
        {
            //Image source handler to return 
            IImageSourceHandler returnValue = null;
            //check the specific source type and return the correct image source handler 
            if (source is UriImageSource)
            {
                returnValue = new ImageLoaderSourceHandler();
            }
            else if (source is FileImageSource)
            {
                returnValue = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource)
            {
                returnValue = new StreamImagesourceHandler();
            }
            return returnValue;
        }
    }
}
