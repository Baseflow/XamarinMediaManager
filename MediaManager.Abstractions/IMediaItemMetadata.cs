using System;
using System.ComponentModel;

namespace Plugin.MediaManager.Abstractions
{
    /// <summary>
    /// MediaItem Metadata. 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public interface IMediaItemMetadata : INotifyPropertyChanged
    {
        /// <summary>
        /// The album title for the media.
        /// </summary>
        string Album { get; set; }

        /// <summary>
        /// The artwork for the album of the media's original source as a Bitmap.
        /// </summary>
        object AlbumArt { get; set; }

        /// <summary>
        /// The artist for the album of the media's original source.
        /// </summary>
        string AlbumArtist { get; set; }

        /// <summary>
        /// The artwork for the album of the media's original source as a Uri style string.
        /// </summary>
        string AlbumArtUri { get; set; }

        /// <summary>
        /// The artwork for the media as a Bitmap.
        /// </summary>
        object Art { get; set; }

        /// <summary>
        /// The artist of the media.
        /// </summary>
        string Artist { get; set; }

        /// <summary>
        /// The artwork for the media as a Uri style string.
        /// </summary>
        string ArtUri { get; set; }

        /// <summary>
        /// The author of the media.
        /// </summary>
        string Author { get; set; }

        /// <summary>
        /// The bluetooth folder type of the media specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        string BluetoothFolderType { get; set; }

        /// <summary>
        /// The compilation status of the media.
        /// </summary>
        string Compilation { get; set; }

        /// <summary>
        /// The composer of the media.
        /// </summary>
        string Composer { get; set; }

        /// <summary>
        /// The date the media was created or published.
        /// </summary>
        DateTime Date { get; set; }

        /// <summary>
        /// The disc number for the media's original source.
        /// </summary>
        int DiscNumber { get; set; }

        /// <summary>
        /// A description that is suitable for display to the user.
        /// </summary>
        string DisplayDescription { get; set; }

        /// <summary>
        /// An icon or thumbnail that is suitable for display to the user.
        /// </summary>
        object DisplayIcon { get; set; }

        /// <summary>
        /// An icon or thumbnail that is suitable for display to the user.
        /// </summary>
        string DisplayIconUri { get; set; }

        /// <summary>
        /// A subtitle that is suitable for display to the user.
        /// </summary>
        string DisplaySubtitle { get; set; }

        /// <summary>
        /// A title that is suitable for display to the user.
        /// </summary>
        string DisplayTitle { get; set; }

        /// <summary>
        /// The duration of the media in ms.
        /// </summary>
        int Duration { get; set; }

        /// <summary>
        /// The genre of the media.
        /// </summary>
        string Genre { get; set; }

        /// <summary>
        /// A string key for identifying the content.
        /// </summary>
        string MediaId { get; set; }

        /// <summary>
        /// A Uri formatted string representing the content.
        /// </summary>
        string MediaUri { get; set; }

        /// <summary>
        /// The number of tracks in the media's original source.
        /// </summary>
        int NumTracks { get; set; }

        /// <summary>
        /// The overall rating for the media.
        /// </summary>
        string Rating { get; set; }

        /// <summary>
        /// The title of the media.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// The track number for the media.
        /// </summary>
        int TrackNumber { get; set; }

        /// <summary>
        /// The user's rating for the media.
        /// </summary>
        string UserRating { get; set; }

        /// <summary>
        /// The writer of the media.
        /// </summary>
        string Writer { get; set; }

        /// <summary>
        /// The year the media was created or published as a long.
        /// </summary>
        long Year { get; set; }
    }
}
