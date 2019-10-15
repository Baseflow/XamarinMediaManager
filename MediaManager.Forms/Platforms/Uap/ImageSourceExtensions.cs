using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

namespace MediaManager.Forms
{
    public static partial class ImageSourceExtensions
    {
        public static ImageSource ToImageSource(this BitmapImage bitmapImage)
        {
            /*TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>();
            Task.Run(async () => {
                IRandomAccessStreamReference random = RandomAccessStreamReference.CreateFromUri(bitmapImage.UriSour‌​ce);
                var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(await random.OpenReadAsync());
                var pixelData = await decoder.GetPixelDataAsync();
                var bitmapData = pixelData.DetachPixelData();
                tcs.SetResult(bitmapData);
            });
            var bitmap = tcs.Task.GetAwaiter().GetResult();
            return ImageSource.FromStream(() => new MemoryStream(bitmap));*/

            return ImageSource.FromUri(bitmapImage.UriSource);
        }

        public static async Task<Windows.UI.Xaml.Media.ImageSource> ToNative(this ImageSource source)
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
                    return new FontImageSourceHandler();
                case UriImageSource _:
                default:
                    return new UriImageSourceHandler();
            }
        }
    }
}
