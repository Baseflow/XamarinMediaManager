using System;
namespace Plugin.MediaManager.Abstractions
{
    public interface IMediaFile
    {
        /// <summary>
        /// Indicator for player which type of file it should play
        /// </summary>
        MediaFileType Type { get; set; }

        /// <summary>
        /// Url to media on the internet or on the file system
        /// </summary>
        string Url { get; set; }
    }
}

