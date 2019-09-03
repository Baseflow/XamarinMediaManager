using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Forms;

namespace MediaManager.Forms.Platforms.Uap
{
    public static class ImageSourceHelper
    {
        public static async Task<ImageSource> ToImageSource(this BitmapImage bitmapImage)
        {
            IRandomAccessStreamReference random = RandomAccessStreamReference.CreateFromUri(bitmapImage.UriSour‌​ce);
            var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(await random.OpenReadAsync());
            var pixelData = await decoder.GetPixelDataAsync();
            var bitmapData = pixelData.DetachPixelData();

            return ImageSource.FromStream(() => new MemoryStream(bitmapData));
        }
    }
}
