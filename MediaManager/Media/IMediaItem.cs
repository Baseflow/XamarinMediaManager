namespace MediaManager.Media
{
    public interface IMediaItem
    {
        /// <summary>
        /// The type of folder that contains folders categorized by Album as specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        int BtFolderTypeAlbums { get; set; }

        /// <summary>
        /// The type of folder that contains folders categorized by artist as specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        int BtFolderTypeArtists { get; set; }

        /// <summary>
        /// The type of folder that contains folders categorized by genre as specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        int BtFolderTypeGenres { get; set; }

        /// <summary>
        /// The type of folder that is unknown or contains media elements of mixed types as specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        int BtFolderTypeMixed { get; set; }

        /// <summary>
        /// The type of folder that contains folders categorized by playlist as specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        int BtFolderTypePlaylists { get; set; }

        /// <summary>
        /// The type of folder that contains media elements only as specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        int BtFolderTypeTitles { get; set; }

        /// <summary>
        /// The type of folder that contains folders categorized by year as specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        int BtFolderTypeYears { get; set; }

        /// <summary>
        /// The metadata key for a int typed value to retrieve the information about whether the media is an advertisement.
        /// </summary>
        string MetadataAdvertisement { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the Album title for the media.
        /// </summary>
        string MetadataAlbum { get; set; }

        /// <summary>
        /// The metadata key for a Bitmap typed value to retrieve the information about the artwork for the Album of the media's original source.
        /// </summary>
        object MetadataAlbumArt { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the artist for the Album of the media's original source.
        /// </summary>
        string MetadataAlbumArtist { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the Uri of the artwork for the Album of the media's original source.
        /// </summary>
        string MetadataAlbumArtUri { get; set; }

        /// <summary>
        /// The metadata key for a Bitmap typed value to retrieve the information about the artwork for the media.
        /// </summary>
        string MetadataArt { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the artist of the media.
        /// </summary>
        string MetadataArtist { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about Uri of the artwork for the media.
        /// </summary>
        string MetadataArtUri { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the author of the media.
        /// </summary>
        string MetadataAuthor { get; set; }

        /// <summary>
        /// The metadata key for a int typed value to retrieve the information about the bluetooth folder type of the media specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        string MetadataBtFolderType { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the compilation status of the media.
        /// </summary>
        string MetadataCompilation { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the composer of the media.
        /// </summary>
        string MetadataComposer { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the date the media was created or published.
        /// </summary>
        string MetadataDate { get; set; }

        /// <summary>
        /// The metadata key for a int typed value to retrieve the information about the disc number for the media's original source.
        /// </summary>
        string MetadataDiscNumber { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the description that is suitable for display to the user.
        /// </summary>
        string MetadataDisplayDescription { get; set; }

        /// <summary>
        /// The metadata key for a Bitmap typed value to retrieve the information about the icon or thumbnail that is suitable for display to the user.
        /// </summary>
        string MetadataDisplayIcon { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the Uri of icon or thumbnail that is suitable for display to the user.
        /// </summary>
        string MetadataDisplayIconUri { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the subtitle that is suitable for display to the user.
        /// </summary>
        string MetadataDisplaySubtitle { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the title that is suitable for display to the user.
        /// </summary>
        string MetadataDisplayTitle { get; set; }

        /// <summary>
        /// The metadata key for a int typed value to retrieve the information about the download status of the media which will be used for later offline playback.
        /// </summary>
        string MetadataDownloadStatus { get; set; }

        /// <summary>
        /// The metadata key for a int typed value to retrieve the information about the duration of the media in ms.
        /// </summary>
        string MetadataDuration { get; set; }

        /// <summary>
        /// A Bundle extra.
        /// </summary>
        string MetadataExtras { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the genre of the media.
        /// </summary>
        string MetadataGenre { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the media ID of the content.
        /// </summary>
        string MetadataMediaId { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the Uri of the content.
        /// </summary>
        string MetadataMediaUri { get; set; }

        /// <summary>
        /// The metadata key for a int typed value to retrieve the information about the number of tracks in the media's original source.
        /// </summary>
        string MetadataNumTracks { get; set; }

        /// <summary>
        /// The metadata key for a Rating2 typed value to retrieve the information about the overall rating for the media.
        /// </summary>
        string MetadataRating { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the title of the media.
        /// </summary>
        string MetadataTitle { get; set; }

        /// <summary>
        /// The metadata key for a int typed value to retrieve the information about the track number for the media.
        /// </summary>
        string MetadataTrackNumber { get; set; }

        /// <summary>
        /// The metadata key for a Rating2 typed value to retrieve the information about the user's rating for the media.
        /// </summary>
        string MetadataUserRating { get; set; }

        /// <summary>
        /// The metadata key for a CharSequence or string typed value to retrieve the information about the writer of the media.
        /// </summary>
        string MetadataWriter { get; set; }

        /// <summary>
        /// The metadata key for a int typed value to retrieve the information about the year the media was created or published.
        /// </summary>
        string MetadataYear { get; set; }

        /// <summary>
        /// The status value to indicate the media item is downloaded for later offline playback.
        /// </summary>
        int StatusDownloaded { get; set; }

        /// <summary>
        /// The status value to indicate the media item is being downloaded.
        /// </summary>
        int StatusDownloading { get; set; }

        /// <summary>
        /// The status value to indicate the media item is not downloaded.
        /// </summary>
        int StatusNotDownloaded { get; set; }
    }
}
