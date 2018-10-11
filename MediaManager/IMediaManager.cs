using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager
{
    public interface IMediaManager<TMediaPlayer, TPlayer> : IMediaManager where TMediaPlayer : class, IMediaPlayer<TPlayer> where TPlayer : class
    {
        new TMediaPlayer MediaPlayer { get; set; }
    }

    public interface IMediaManager : IPlaybackManager
    {
        IMediaPlayer MediaPlayer { get; set; }

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        Dictionary<string, string> RequestHeaders { get; set; }

        //IAudioPlayer AudioPlayer { get; set; }

        //IVideoPlayer VideoPlayer { get; set; }

        //INotificationManager NotificationManager { get; set; }

        IMediaExtractor MediaExtractor { get; set; }

        IVolumeManager VolumeManager { get; set; }

        IMediaQueue MediaQueue { get; set; }

        void Init();

        // <summary>
        /// Plays a media item
        /// </summary>
        Task Play(IMediaItem mediaItem);

        /// <summary>
        /// Plays an uri that can be both remote or local
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<IMediaItem> Play(string uri);

        /// <summary>
        /// Plays a list of media items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Task Play(IEnumerable<IMediaItem> items);

        /// <summary>
        /// Plays a list of uri's
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items);

        /// <summary>
        /// Plays a file from the local file system
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<IMediaItem> Play(FileInfo file);

        /// <summary>
        /// Plays all files inside the directory
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <returns></returns>
        Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo);
    }
}
