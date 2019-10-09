using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace MediaManager.Forms.Platforms.Android
{
    public static class ImageSourceHelper
    {
        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            if (bitmap != null)
            {
                var stream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                var bitmapData = stream.ToArray();
                return ImageSource.FromStream(() => new MemoryStream(bitmapData));
            }
            return null;
        }

        public static async Task<Bitmap> ToNative(this ImageSource source, Context context)
        {
            var imageHandler = source.GetHandler();
            if (imageHandler == null)
                return null;

            var nativeImage = await imageHandler.LoadImageAsync(source, context);
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
