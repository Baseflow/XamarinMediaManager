using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Player;
using MediaManager.Queue;

namespace MediaManager.Playback
{
    public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs e);
    public delegate void BufferedChangedEventHandler(object sender, BufferedChangedEventArgs e);
    public delegate void PositionChangedEventHandler(object sender, PositionChangedEventArgs e);

    public delegate void MediaItemFinishedEventHandler(object sender, MediaItemEventArgs e);
    public delegate void MediaItemChangedEventHandler(object sender, MediaItemEventArgs e);
    public delegate void MediaItemFailedEventHandler(object sender, MediaItemFailedEventArgs e);

    public interface IPlaybackManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Managing the step size for the step forward and step backward functions
        /// </summary>
        TimeSpan StepSize { get; set; }


        /// <summary>
        /// Reading the current status of the player
        /// </summary>
        MediaPlayerState State { get; }

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

        float Speed { get; set; }

        RepeatMode RepeatMode { get; set; }

        ShuffleMode ShuffleMode { get; set; }

        bool ClearQueueOnPlay { get; set; }

        /// <summary>
        /// Plays the current MediaItem
        /// </summary>
        Task Play();

        /// <summary>
        /// Pauses the current MediaItem
        /// </summary>
        Task Pause();

        /// <summary>
        /// Stops playing
        /// </summary>
        Task Stop();

        /// <summary>
        /// Plays the previous MediaItem
        /// </summary>
        /// <returns>Playing previous MediaItem was possible</returns>
        Task<bool> PlayPrevious();

        /// <summary>
        /// Plays the next MediaItem
        /// </summary>
        /// <returns>Playing next MediaItem was possible</returns>
        Task<bool> PlayNext();

        /// <summary>
        /// Will try to play a specific item from the Queue
        /// </summary>
        /// <param name="mediaItem"></param>
        /// <returns>false when the item doesn't exist in the Queue</returns>
        Task<bool> PlayQueueItem(IMediaItem mediaItem);

        /// <summary>
        /// Will try to play a specific item from the Queue
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<bool> PlayQueueItem(int index);

        /// <summary>
        /// Seeks forward a fixed amount of seconds of the current MediaItem
        /// </summary>
        Task StepForward();

        /// <summary>
        /// Seeks backward a fixed amount of seconds of the current MediaItem
        /// </summary>
        Task StepBackward();

        /// <summary>
        /// Seeks to the specified amount of seconds
        /// </summary>
        /// <param name="position"></param>
        Task SeekTo(TimeSpan position);

        event StateChangedEventHandler StateChanged;

        event BufferedChangedEventHandler BufferedChanged;

        event PositionChangedEventHandler PositionChanged;

        event MediaItemFinishedEventHandler MediaItemFinished;

        event MediaItemChangedEventHandler MediaItemChanged;

        event MediaItemFailedEventHandler MediaItemFailed;
    }
}
