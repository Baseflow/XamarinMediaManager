using System.IO;
using Android.Graphics;
using Xamarin.Forms;

namespace MediaManager.Forms.Platforms.Android
{
    public static class ImageSourceHelper
    {
        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            if (bitmap != null)
            {
                MemoryStream stream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                byte[] bitmapData = stream.ToArray();
                return ImageSource.FromStream(() => new MemoryStream(bitmapData));
            }
            return null;
        }
    }
}
