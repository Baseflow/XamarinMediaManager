using System;
using System.Threading.Tasks;
using Android.Graphics;
using MediaManager.Library;
using MediaManager.Media;

namespace MediaManager.Platforms.Android.Media
{
    public class UriImageProvider : IImageProvider
    {
        public async Task<object> ProvideImage(IMediaItem mediaItem)
        {
            object image = null;
            if (!string.IsNullOrEmpty(mediaItem.ArtUri))
            {
                try
                {
                    var url = new Java.Net.URL(mediaItem.ArtUri);
                    image = await Task.Run(() => BitmapFactory.DecodeStreamAsync(url.OpenStream()));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return image;
        }
    }
}
