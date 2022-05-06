using CoreGraphics;
using MediaManager.Library;
using MediaPlayer;

namespace MediaManager.Platforms.Ios.Media
{
    public static class MPMediaItemExtensions
    {
        public static IMediaItem ToMediaItem(this MPMediaItem item)
        {
            if (item == null)
                return null;
#if IOS
            var output = new MediaItem
            {
                MediaType = item.MediaType.ToMediaType(),
                Album = item.AlbumTitle,
                Artist = item.Artist,
                Compilation = null,
                Composer = item.Composer,
                Duration = TimeSpan.FromSeconds(item.PlaybackDuration),
                Genre = item.Genre,
                Title = item.Title,
                AlbumArtist = item.AlbumArtist,
                DiscNumber = item.DiscNumber,
                MediaUri = item.AssetURL.ToString(),
                NumTracks = item.AlbumTrackCount,
                UserRating = item.Rating,
                Id = item.PersistentID.ToString()
            };

            if (item.ReleaseDate != null)
                output.Date = (DateTime)item.ReleaseDate;

            if (item.Artwork != null)
                output.Image = item.Artwork.ImageWithSize(new CGSize(300, 300));

            if (output.Date != null)
                output.Year = output.Date.Year;
#elif TVOS
            var output = new MediaItem();
            //TODO: something like this?
            /*
            output.MediaType = item.ValueForKey(MPMediaItem.MediaTypeProperty);
            output.Album = item.ValueForProperty(MPMediaItem.AlbumTitleProperty).ToString();
            output.Artist = item.Artist,
            output.Compilation = null,
            output.Composer = item.Composer,
            output.Duration = TimeSpan.FromSeconds(item.PlaybackDuration),
            output.Genre = item.Genre,
            output.Title = item.Title,
            output.AlbumArtist = item.AlbumArtist,
            output.DiscNumber = item.DiscNumber,
            output.MediaUri = item.AssetURL.ToString(),
            output.NumTracks = item.AlbumTrackCount,
            output.UserRating = item.Rating,
            output.Id = item.PersistentID.ToString()*/

#endif
            return output;
        }

        public static IEnumerable<IMediaItem> ToMediaItems(this IEnumerable<MPMediaItem> items)
        {
#if IOS
            return items
                .Where(i => i.AssetURL != null && i.IsCloudItem == false && i.HasProtectedAsset == false)
                .Select(i => i.ToMediaItem());
#elif TVOS
            return items
                .Select(i => i.ToMediaItem());
#endif
        }
    }
}
