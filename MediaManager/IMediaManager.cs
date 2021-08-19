using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MediaManager.Library;
using MediaManager.Media;
using MediaManager.Notifications;
using MediaManager.Playback;
using MediaManager.Player;
using MediaManager.Queue;
using MediaManager.Volume;

namespace MediaManager
{
    public interface IMediaManager<TPlayer> : IMediaManager where TPlayer : class
    {
        TPlayer Player { get; }
    }

    public interface IMediaManager : IPlaybackManager, IDisposable
    {
        IMediaPlayer MediaPlayer { get; set; }

        IMediaLibrary Library { get; set; }

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        Dictionary<string, string> RequestHeaders { get; set; }

        INotificationManager Notification { get; set; }

        IVolumeManager Volume { get; set; }

        IMediaExtractor Extractor { get; set; }

        IMediaQueue Queue { get; set; }

        void Init();

        /// <summary>
        /// Plays a media item
        /// </summary>
        Task<IMediaItem> Play(IMediaItem mediaItem);

        /// <summary>
        /// Plays an uri that can be both remote or local
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<IMediaItem> Play(string uri);

        /// <summary>
        /// Plays an embeded resource
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        Task<IMediaItem> PlayFromAssembly(string resourceName, Assembly assembly = null);

        /// <summary>
        /// Plays a native resource
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        Task<IMediaItem> PlayFromResource(string resourceName);

        /// <summary>
        /// Plays a list of media items
        /// </summary>
        /// <param name="mediaItems"></param>
        /// <returns></returns>
        Task<IMediaItem> Play(IEnumerable<IMediaItem> mediaItems);

        /// <summary>
        /// Plays a list of uri's
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<IMediaItem> Play(IEnumerable<string> items);

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
        Task<IMediaItem> Play(DirectoryInfo directoryInfo);

        /// <summary>
        /// Plays media from a Stream. The cacheName name must be a valid media name, like: something.mp4
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        Task<IMediaItem> Play(Stream stream, string cacheName);
        Task<IMediaItem> Play(Stream stream);
    }
}
