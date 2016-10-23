using System;
using System.ComponentModel;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager.Abstractions
{
    public interface IMediaFile : INotifyPropertyChanged
    {
        /// <summary>
        /// Indicator for player which type of file it should play
        /// </summary>
        MediaFileType Type { get; set; }

        /// <summary>
        /// Url to media on the internet or on the file system
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// Object that contains the cover
        /// </summary>
        object Cover { get; set; }
    }
}

