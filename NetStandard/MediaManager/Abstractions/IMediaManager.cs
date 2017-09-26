using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Abstractions.Enums;
using MediaManager.Abstractions.EventArguments;

namespace MediaManager.Abstractions
{
    //public delegate void MediaFileChangedEventHandler(object sender, MediaFileChangedEventArgs e);

    //public delegate void MediaFileFailedEventHandler(object sender, MediaFileFailedEventArgs e);

    public interface IMediaManager
    {
        /// <summary>
        /// Player responsible for audio playback
        /// </summary>
        IAudioPlayer AudioPlayer { get; set; }

        /// <summary>
        /// Player responsible for video playback
        /// </summary>
        IVideoPlayer VideoPlayer { get; set; }

        IPlaybackManager CurrentPlaybackManager { get; }

        /// <summary>
        /// Queue to play media in sequences
        /// </summary>
        IMediaQueue MediaQueue { get; set; }

        /// <summary>
        /// Extracts media information to put it into an IMediaFile
        /// </summary>
        IMediaExtractor MediaExtractor { get; set; }

        /// <summary>
        /// Manages notifications to the native system
        /// </summary>
        INotificationManager NotificationManager { get; set; }

        /// <summary>
        /// Used to manage the volume
        /// </summary>
        IVolumeManager VolumeManager { get; set; }

        /// <summary>
        /// Used in various views to control the playback
        /// </summary>
        IPlaybackController PlaybackController { get; set; }

        Task Play(string url, MediaItemType type);

        Task Play(IMediaItem item);

        Task Play(IEnumerable<IMediaItem> items);

        Task Play(Stream stream, MediaItemType type);

        Task Play(FileInfo file, MediaItemType type);
    }
}
