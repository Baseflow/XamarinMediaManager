using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager.Abstractions
{
    public delegate void MetadataUpdatedEventHandler(object sender, MetadataChangedEventArgs e);

    /// <summary>
    /// Information about the mediafile
    /// </summary>
    public interface IMediaFile
    {
        /// <summary>
        /// Indicator for player which type of file it should play
        /// </summary>
        MediaFileType Type { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        IMediaFileMetadata Metadata { get; set; }

        /// <summary>
        /// Raised when mediadata of MediaFile failed to update
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

