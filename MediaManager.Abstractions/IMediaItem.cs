using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager.Abstractions
{
    public delegate void MetadataUpdatedEventHandler(object sender, MetadataChangedEventArgs e);

    /// <summary>
    /// Information about the MediaItem
    /// </summary>
    public interface IMediaItem
    {
        /// <summary>
        /// Indicator for player which type of file it should play
        /// </summary>
        MediaItemType Type { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        IMediaItemMetadata Metadata { get; set; }

        /// <summary>
        /// Raised when mediadata of MediaItem failed to update
        /// </summary>
        event MetadataUpdatedEventHandler MetadataUpdated;

        /// <summary>
        /// Gets or sets the URL. Can be on the internet or local storage
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [metadata extracted].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [metadata extracted]; otherwise, <c>false</c>.
        /// </value>
        bool MetadataExtracted { get; set; }
    }
}

