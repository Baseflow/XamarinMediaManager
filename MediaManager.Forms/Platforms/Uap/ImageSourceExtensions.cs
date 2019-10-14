using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

namespace MediaManager.Forms
{
    public static partial class ImageSourceExtensions
    {
        public static async Task<ImageSource> ToImageSource(this BitmapImage bitmapImage)
        {
            IRandomAccessStreamReference random = RandomAccessStreamReference.CreateFromUri(bitmapImage.UriSour‌​ce);
            var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(await random.OpenReadAsync());
            var pixelData = await decoder.GetPixelDataAsync();
            var bitmapData = pixelData.DetachPixelData();

            return ImageSource.FromStream(() => new MemoryStream(bitmapData));
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
                    return new FontImageSourceHandler();
                case UriImageSource _:
                default:
                    return new UriImageSourceHandler();
            }
        }
    }
}
