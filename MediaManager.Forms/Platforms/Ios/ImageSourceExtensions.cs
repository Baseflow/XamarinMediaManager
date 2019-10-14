using System.Threading.Tasks;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace MediaManager.Forms
{
    public static partial class ImageSourceExtensions
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
            var imageHandler = source.GetImageSourceHandler();
            if (imageHandler == null)
                return null;

            return await imageHandler.LoadImageAsync(source).ConfigureAwait(false);
        }

        public static IImageSourceHandler GetImageSourceHandler(this ImageSource source)
        {
            //check the specific source type and return the correct image source handler 
            switch (source)
            {
                case FileImageSource _:
                    return new FileImageSourceHandler();
                case StreamImageSource _:
                    return new StreamImagesourceHandler();
                case FontImageSource _:
                    return new FontImageSourceHandler();
                case UriImageSource _:
                default:
                    return new ImageLoaderSourceHandler();
            }
        }
    }
}
