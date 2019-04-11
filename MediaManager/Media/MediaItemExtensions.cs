using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager.Media
{
    public static class MediaItemExtensions
    {
        public static string GetTitle(this IMediaItem mediaItem)
        {
            if (!string.IsNullOrEmpty(mediaItem.DisplayTitle))
                return mediaItem.Title;
            else if (!string.IsNullOrEmpty(mediaItem.Title))
                return mediaItem.Title;
            else
                return "";
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
                return "";
        }
    }
}
