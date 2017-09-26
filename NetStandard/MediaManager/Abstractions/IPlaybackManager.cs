using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Abstractions.Enums;
using MediaManager.Abstractions.EventArguments;

namespace MediaManager.Abstractions
{
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);

    public delegate void PlayingChangedEventHandler(object sender, PlayingChangedEventArgs e);

    public delegate void BufferingChangedEventHandler(object sender, BufferingChangedEventArgs e);

    public delegate void MediaFinishedEventHandler(object sender, MediaFinishedEventArgs e);

    public delegate void MediaFailedEventHandler(object sender, MediaFailedEventArgs e);

    public interface IPlaybackManager
    {
        /// <summary>
        /// Raised when the status changes
        /// </summary>
        event StatusChangedEventHandler Status;

        /// <summary>
        /// Raised at least every second when the player is playing.
        /// </summary>
        event PlayingChangedEventHandler Playing;

        /// <summary>
        /// Raised each time the buffering is updated by the player.
        /// </summary>
        event BufferingChangedEventHandler Buffering;

        /// <summary>
        /// Raised when media is finished playing.
        /// </summary>
        event MediaFinishedEventHandler Finished;

        /// <summary>
        /// Raised when media is failed playing.
        /// </summary>
        event MediaFailedEventHandler Failed;

        /// <summary>
        /// Reading the current state of the player
        /// </summary>
        PlaybackState State { get; }

        /// <summary>
        /// Gets the players position
        /// </summary>
        TimeSpan Position { get; }

        /// <summary>
        /// Gets the source duration
        /// If the response is TimeSpan.Zero, the duration is unknown or the player is still buffering.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// Gets the buffered time
        /// </summary>
        TimeSpan Buffered { get; }

        /// <summary>
        /// Creates new MediaFile object, adds it to the queue and starts playing
        /// </summary>
        Task Play(string url);

        Task Play(FileInfo file);

        Task Play(Stream stream);

        /// <summary>
        /// Plays the media item
        /// </summary>
        Task Play(IMediaItem item);

        /// <summary>
        /// Stops playing but retains position
        /// </summary>
        Task Pause();
        
        /// <summary>
        /// Stops playing
        /// </summary>
        Task Stop();

        /// <summary>
        /// Changes position to the specified number of milliseconds from zero
        /// </summary>
        Task Seek(TimeSpan position);

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        Dictionary<string, string> RequestHeaders { get; set; }
    }
}
