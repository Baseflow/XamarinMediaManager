using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using MediaManager.Library;
using MediaPlayer;

namespace MediaManager.Platforms.Ios.Media
{
    public static class MPMediaItemExtensions
    {
        public static IMediaItem ToMediaItem(this MPMediaItem item)
        {
            var output = new MediaItem
            {
                MediaType = MediaType.Audio,
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
            
            return output;
        }
        
        public static IEnumerable<IMediaItem> ToMediaItems(this IEnumerable<MPMediaItem> items)
        {
            return items
                .Where(i => i.AssetURL != null && i.IsCloudItem == false && i.HasProtectedAsset == false)
                .Select(i => i.ToMediaItem());
        }
    }
}
