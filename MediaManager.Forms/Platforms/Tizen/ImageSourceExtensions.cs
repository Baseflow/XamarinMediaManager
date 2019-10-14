using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;

namespace MediaManager.Forms
{
    public static partial class ImageSourceExtensions
    {
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
