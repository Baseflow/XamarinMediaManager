using Android.Content.Res;
using Android.Graphics;
using MediaManager.Library;
using MediaManager.Media;

namespace MediaManager.Platforms.Android.Media
{
    public class ResourceImageProvider : MediaExtractorProviderBase, IMediaItemImageProvider
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;
        protected Resources Resources => Resources.System;

        public async Task<object> ProvideImage(IMediaItem mediaItem)
        {
            object image = null;
            try
            {
                var artId = int.MinValue;
                if (!string.IsNullOrEmpty(mediaItem.ImageUri) && int.TryParse(mediaItem.ImageUri, out artId))
                { }
                else if (!string.IsNullOrEmpty(mediaItem.AlbumImageUri) && artId == int.MinValue && int.TryParse(mediaItem.AlbumImageUri, out artId))
                { }

                if (artId != int.MinValue)
                    image = await BitmapFactory.DecodeResourceAsync(Resources, artId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return mediaItem.Image = image;
        }
    }
}
