namespace MediaManager.Library
{
    public static class MediaItemExtensions
    {
        public static IDictionary<string, object> ToDictionary(this IMediaItem mediaItem)
        {
            var mediaItemDict = new Dictionary<string, object>();
            foreach (var item in mediaItem.GetType().GetProperties())
            {
                var value = item.GetValue(mediaItem, null);
                if (value != null)
                    mediaItemDict.Add(item.Name, value);
            }
            return mediaItemDict;
        }

        public static string GetTitle(this IMediaItem mediaItem)
        {
            if (!string.IsNullOrEmpty(mediaItem.DisplayTitle))
                return mediaItem.DisplayTitle;
            else if (!string.IsNullOrEmpty(mediaItem.Title))
                return mediaItem.Title;
            else
                return string.Empty;
        }

        public static string GetContentTitle(this IMediaItem mediaItem)
        {
            if (!string.IsNullOrEmpty(mediaItem.DisplaySubtitle))
                return mediaItem.DisplaySubtitle;
            else if (!string.IsNullOrEmpty(mediaItem.Artist))
                return mediaItem.Artist;
            else if (!string.IsNullOrEmpty(mediaItem.AlbumArtist))
                return mediaItem.AlbumArtist;
            else if (!string.IsNullOrEmpty(mediaItem.Album))
                return mediaItem.Album;
            else
                return string.Empty;
        }

        public static string GetSubText(this IMediaItem mediaItem)
        {
            if (!string.IsNullOrEmpty(mediaItem.Album))
                return mediaItem.Album;
            else if (!string.IsNullOrEmpty(mediaItem.Artist))
                return mediaItem.Artist;
            else if (!string.IsNullOrEmpty(mediaItem.AlbumArtist))
                return mediaItem.AlbumArtist;
            else
                return string.Empty;
        }
    }
}
