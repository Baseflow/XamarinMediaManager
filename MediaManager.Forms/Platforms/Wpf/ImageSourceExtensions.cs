using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

namespace MediaManager.Forms
{
    public static partial class ImageSourceExtensions
    {
        public static async Task<System.Windows.Media.ImageSource> ToNative(this ImageSource source)
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
                    return new StreamImageSourceHandler();
                case FontImageSource _:
                case UriImageSource _:
                default:
                    return new UriImageSourceHandler();
            }
        }
    }
}
