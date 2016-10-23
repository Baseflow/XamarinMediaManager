using System;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager.Abstractions
{
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);

    public delegate void PlayingChangedEventHandler(object sender, PlayingChangedEventArgs e);

    public delegate void BufferingChangedEventHandler(object sender, BufferingChangedEventArgs e);

    public delegate void MediaFinishedEventHandler(object sender, MediaFinishedEventArgs e);

    public delegate void MediaFailedEventHandler(object sender, MediaFailedEventArgs e);

    /// <summary>
    /// The main purpose of this class is to be a controlling unit for all the single MediaItem implementations, who
    /// in themselve can play their media, but need a central controling unit, surrounding them
    /// </summary>
    public interface IMediaManager
    {
        /// <summary>
        /// Player responseble for audio playback
        /// </summary>
        IAudioPlayer AudioPlayer { get; set; }

        /// <summary>
        /// Player responseble for video playback
        /// </summary>
        IVideoPlayer VideoPlayer { get; set; }

        /// <summary>
        /// Queue to play media in sequences
        /// </summary>
        IMediaQueue MediaQueue { get; set; }

        /// <summary>
        /// Manages notifications to the native system
        /// </summary>
        IMediaNotificationManager  MediaNotificationManager { get; set; }

        /// <summary>
        /// Extracts media information to put it into an IMediaFile
        /// </summary>
        IMediaExtractor MediaExtractor { get; set; }

        /// <summary>
        /// Reading the current status of the player
        /// </summary>
        MediaPlayerStatus Status { get; }

        /// <summary>
        /// Gets the players position in milliseconds
        /// </summary>
        TimeSpan Position { get; }

        /// <summary>
        /// Gets the source duration in milliseconds
        /// If the response is -1, the duration is unknown or the player is still buffering.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// Gets the buffered time in milliseconds
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
        /// Starts playing from the current position
        /// </summary>
        Task Play(IMediaFile mediaFile);

        /// <summary>
        /// Starts playing from the current position
        /// </summary>
        Task Play(string url);

        /// <summary>
        /// Start playing if nothing is playing, otherwise it pauses the current media
        /// </summary>
        Task PlayPause();

        /// <summary>
        /// Stops playing but retains position
        /// </summary>
        Task Pause();

        /// <summary>
        /// Should be the same as calling PlayByPosition(Queue.size()+1)
        /// Maybe you'll want to preload the next song into memory ...
        /// </summary>
        Task PlayNext();

        /// <summary>
        /// Should be the same as calling PlayByPosition(Queue.size()-1).
        /// Maybe you'll want to keep the last song in memory ...
        /// </summary>
        Task PlayPrevious();

        /// <summary>
        /// Start playing a track by its position in the Queue
        /// </summary>
        Task PlayByPosition(int index);

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
