using System.Threading.Tasks;
using AppKit;
using CoreGraphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

namespace MediaManager.Forms
{
    public static partial class ImageSourceExtensions
    {
        public static ImageSource ToImageSource(this CGImage cgImage)
        {
            return ImageSource.FromStream(() => new NSImage(cgImage, new CGSize(cgImage.Width, cgImage.Height)).AsTiff().AsStream());
        }

        public static async Task<NSImage> ToNative(this ImageSource source)
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
                case UriImageSource _:
                default:
                    return new ImageLoaderSourceHandler();
            }
        }
    }
}
