using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager.Media
{
    public class MediaItem : IMediaItem
    {
        public MediaItem(string uri)
        {
            MediaUri = uri;
        }

        public bool IsMetadataExtracted { get; set; } = false;
        public string Advertisement { get; set; }
        public string Album { get; set; }
        public object AlbumArt { get; set; }
        public string AlbumArtist { get; set; }
        public string AlbumArtUri { get; set; }
        public object Art { get; set; }
        public string Artist { get; set; }
        public string ArtUri { get; set; }
        public string Author { get; set; }
        public BtFolderType BtFolderType { get; set; }
        public string Compilation { get; set; }
        public string Composer { get; set; }
        public string Date { get; set; }
        public int DiscNumber { get; set; }
        public string DisplayDescription { get; set; }
        public object DisplayIcon { get; set; }
        public string DisplayIconUri { get; set; }
        public string DisplaySubtitle { get; set; }
        public string DisplayTitle { get; set; }
        public DownloadStatus DownloadStatus { get; set; }
        public TimeSpan Duration { get; set; }
        public object Extras { get; set; }
        public string Genre { get; set; }
        public string MediaId { get; set; }
        public string MediaUri { get; set; }
        public int NumTracks { get; set; }
        public object Rating { get; set; }
        public string Title { get; set; }
        public int TrackNumber { get; set; }
        public object UserRating { get; set; }
        public string Writer { get; set; }
        public int Year { get; set; }
    }
}
