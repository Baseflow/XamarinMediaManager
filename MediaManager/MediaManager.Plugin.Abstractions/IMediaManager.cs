using System;
using System.Threading.Tasks;

namespace MediaManager.Plugin.Abstractions
{
    public delegate void StatusChangedEventHandler(object sender, EventArgs e);

    public delegate void CoverReloadedEventHandler(object sender, EventArgs e);

    public delegate void PlayingEventHandler(object sender, EventArgs e);

    public delegate void BufferingEventHandler(object sender, EventArgs e);

    public delegate void TrackFinishedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// The main purpose of this class is to be a controlling unit for all the single MediaItem implementations, who
    /// in themselve can play their media, but need a central controling unit, surrounding them
    /// </summary>
    public interface IMediaManager
    {
        /// <summary>
        /// Reading the current status of the player (STOPPED, PAUSED, PLAYING, LOADING - initialization and buffering is combined here)
        /// </summary>
        PlayerStatus Status { get; }

        /// <summary>
        /// Raised when the status changes (playing, pause, buffering)
        /// </summary>
        event StatusChangedEventHandler StatusChanged;

        /// <summary>
        /// Raised when the cover on the player changes
        /// </summary>
        event CoverReloadedEventHandler CoverReloaded;

        /// <summary>
        /// Raised at least every second when the player is playing.
        /// </summary>
        event PlayingEventHandler Playing;

        /// <summary>
        /// Raised each time the buffering is updated by the player.
        /// </summary>
        event BufferingEventHandler Buffering;

        /// <summary>
        /// Raised when a track is finished playing.
        /// </summary>
        event TrackFinishedEventHandler TrackFinished;

        /// <summary>
        /// Starts playing from the current position
        /// </summary>
        Task Play(string url);

        /// <summary>
        /// Stops playing
        /// </summary>
        Task Stop();

        /// <summary>
        /// Stops playing but retains position
        /// </summary>
        Task Pause();

        /// <summary>
        /// Gets the players position in milliseconds
        /// </summary>
        int Position { get; }

        /// <summary>
        /// Gets the source duration in milliseconds
        /// If the response is -1, the duration is unknown or the player is still buffering.
        /// </summary>
        int Duration { get; }

        /// <summary>
        /// Gets the buffered time in milliseconds
        /// </summary>
        int Buffered { get; }

        /// <summary>
        /// Gets the current cover. The class for the instance depends on the platform.
        /// Returns NULL if unknown.
        /// </summary>
        object Cover { get; }

        /// <summary>
        /// Changes position to the specified number of milliseconds from zero
        /// </summary>
        Task Seek(int position);

        /// <summary>
        /// Should be the same as calling PlayByPosition(Queue.size()+1)
        /// Maybe you'll want to preload the next song into memory ...
        /// </summary>
        Task PlayNext(string url);

        /// <summary>
        /// Start playing if nothing is playing, otherwise it pauses the current media
        /// </summary>
        Task PlayPause();

        /// <summary>
        /// Should be the same as calling PlayByPosition(Queue.size()-1).
        /// Maybe you'll want to keep the last song in memory ...
        /// </summary>
        Task PlayPrevious(string url);

        /// <summary>
        /// Start playing a track by its position in the Queue
        /// </summary>
        //Task PlayByPosition(int index);
    }
}
