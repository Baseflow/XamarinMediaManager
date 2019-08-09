using System.Threading.Tasks;
using Android.Content.Res;
using Android.Graphics;
using MediaManager.Library;
using MediaManager.Media;

namespace MediaManager.Platforms.Android.Media
{
    public class ResourceImageProvider : IImageProvider
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;
        protected Resources Resources => Resources.System;

        public Task<object> ProvideImage(IMediaItem mediaItem)
        {
            object image = null;
            try
            {
                int artId = int.MinValue;
                int.TryParse(mediaItem.ArtUri, out artId);

                if (artId == int.MinValue)
                    int.TryParse(mediaItem.AlbumArtUri, out artId);

                if (artId != int.MinValue)
                    image = BitmapFactory.DecodeResource(Resources, artId);

                //if(image == null)
                //    image = BitmapFactory.DecodeResource(Resources, MediaManager.NotificationIconResource);
            }
            catch
            {

            }
            return Task.FromResult(image);
        }
    }
}
