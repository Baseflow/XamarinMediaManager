using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager.Media
{
    public class MediaItem : IMediaItem
    {
        public int BtFolderTypeAlbums { get; set; }
        public int BtFolderTypeArtists { get; set; }
        public int BtFolderTypeGenres { get; set; }
        public int BtFolderTypeMixed { get; set; }
        public int BtFolderTypePlaylists { get; set; }
        public int BtFolderTypeTitles { get; set; }
        public int BtFolderTypeYears { get; set; }
        public string MetadataAdvertisement { get; set; }
        public string MetadataAlbum { get; set; }
        public string MetadataAlbumArt { get; set; }
        public string MetadataAlbumArtist { get; set; }
        public string MetadataAlbumArtUri { get; set; }
        public string MetadataArt { get; set; }
        public string MetadataArtist { get; set; }
        public string MetadataArtUri { get; set; }
        public string MetadataAuthor { get; set; }
        public string MetadataBtFolderType { get; set; }
        public string MetadataCompilation { get; set; }
        public string MetadataComposer { get; set; }
        public string MetadataDate { get; set; }
        public string MetadataDiscNumber { get; set; }
        public string MetadataDisplayDescription { get; set; }
        public string MetadataDisplayIcon { get; set; }
        public string MetadataDisplayIconUri { get; set; }
        public string MetadataDisplaySubtitle { get; set; }
        public string MetadataDisplayTitle { get; set; }
        public string MetadataDownloadStatus { get; set; }
        public string MetadataDuration { get; set; }
        public string MetadataExtras { get; set; }
        public string MetadataGenre { get; set; }
        public string MetadataMediaId { get; set; }
        public string MetadataMediaUri { get; set; }
        public string MetadataNumTracks { get; set; }
        public string MetadataRating { get; set; }
        public string MetadataTitle { get; set; }
        public string MetadataTrackNumber { get; set; }
        public string MetadataUserRating { get; set; }
        public string MetadataWriter { get; set; }
        public string MetadataYear { get; set; }
        public int StatusDownloaded { get; set; }
        public int StatusDownloading { get; set; }
        public int StatusNotDownloaded { get; set; }
    }
}
