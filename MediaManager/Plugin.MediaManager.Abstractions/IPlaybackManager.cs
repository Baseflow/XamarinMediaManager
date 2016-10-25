using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager.Abstractions
{
    public interface IPlaybackManager
    {
        /// <summary>
        /// Reading the current status of the player
        /// </summary>
        MediaPlayerStatus Status { get; }

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
        /// Raised when the status changes
        /// </summary>
        event StatusChangedEventHandler StatusChanged;

        /// <summary>
        /// Raised at least every second when the player is playing.
        /// </summary>
        event PlayingChangedEventHandler PlayingChanged;

        /// <summary>
        /// Raised each time the buffering is updated by the player.
        /// </summary>
        event BufferingChangedEventHandler BufferingChanged;

        /// <summary>
        /// Raised when media is finished playing.
        /// </summary>
        event MediaFinishedEventHandler MediaFinished;

        /// <summary>
        /// Raised when media is failed playing.
        /// </summary>
        event MediaFailedEventHandler MediaFailed;

        /// <summary>
        /// Raised when metadata of MediaFile is changed
        /// </summary>
        event MediaFileChangedEventHandler MediaFileUpdated;

        /// <summary>
        /// Raised when mediadata of MediaFile failed to update
        /// </summary>
        event MediaFileFailedEventHandler MediaFileFailed;

        /// <summary>
        /// Adds MediaFile to the Queue and starts playing
        /// </summary>
        Task Play(IMediaFile mediaFile);

        /// <summary>
        /// Adds all MediaFiles to the Queue and starts playing the first item
        /// </summary>
        Task Play(IEnumerable<IMediaFile> mediaFiles);

        /// <summary>
        /// Creates new MediaFile object, adds it to the queue and starts playing
        /// </summary>
        Task Play(string url, MediaFileType fileType);

        /// <summary>
        /// Start playing if nothing is playing, otherwise it pauses the current media
        /// </summary>
        Task PlayPause();

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
    }
}