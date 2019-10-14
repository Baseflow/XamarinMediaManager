using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace MediaManager.Forms
{
    public static partial class ImageSourceExtensions
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

        public static async Task<Bitmap> ToNative(this ImageSource source)
        {
            return await source.ToNative(Android.App.Application.Context).ConfigureAwait(false);
        }

        public static async Task<Bitmap> ToNative(this ImageSource source, Context context)
        {
            var imageHandler = source.GetImageSourceHandler();
            if (imageHandler == null)
                return null;

            return await imageHandler.LoadImageAsync(source, context).ConfigureAwait(false);
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
