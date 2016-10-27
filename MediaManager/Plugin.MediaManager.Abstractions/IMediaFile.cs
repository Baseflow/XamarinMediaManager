using System;
using System.ComponentModel;

namespace Plugin.MediaManager.Abstractions
{
    /// <summary>
    /// Information about the mediafile
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public interface IMediaFile : INotifyPropertyChanged
    {
        /// <summary>
        /// Indicator for player which type of file it should play
        /// </summary>
        Implementations.MediaFileType Type { get; set; }

        /// <summary>
        /// Url to media on the internet or on the file system
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// Object that contains the cover
        /// </summary>
        object Cover { get; set; }

        /// <summary>
        /// The performing artist if available
        /// </summary>
        string Artist { get; set; }


        /// <summary>
        /// The album title of the mediafile.
        /// </summary>
        string Album { get; set; }

        /// <summary>
        /// The media title if available
        /// Defaults to filename
        /// </summary>
        string Title { get; set; }
    }
}

