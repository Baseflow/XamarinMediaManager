using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager.Playback
{
    public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs e);
    public delegate void PlayingChangedEventHandler(object sender, PlayingChangedEventArgs e);
    public delegate void BufferingChangedEventHandler(object sender, BufferingChangedEventArgs e);
    public delegate void PositionChangedEventHandler(object sender, PositionChangedEventArgs e);
    public delegate void MediaItemFinishedEventHandler(object sender, MediaItemEventArgs e);
    public delegate void MediaItemChangedEventHandler(object sender, MediaItemEventArgs e);
    public delegate void MediaItemFailedEventHandler(object sender, MediaItemFailedEventArgs e);

    public interface IPlaybackManager : INotifyPropertyChanged
    {
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

        /// <summary>
        /// Plays the current MediaFile
        /// </summary>
        Task Play();

        /// <summary>
        /// Pauses the current MediaFile
        /// </summary>
        Task Pause();

        /// <summary>
        /// Stops playing
        /// </summary>
        Task Stop();

        /// <summary>
        /// Plays the previous MediaFile
        /// </summary>
        Task PlayPrevious();

        /// <summary>
        /// Plays the next MediaFile
        /// </summary>
        /// <returns></returns>
        Task<bool> PlayNext();

        /// <summary>
        /// Seeks forward a fixed amount of seconds of the current MediaFile
        /// </summary>
        Task StepForward();

        /// <summary>
        /// Seeks backward a fixed amount of seconds of the current MediaFile
        /// </summary>
        Task StepBackward();

        /// <summary>
        /// Seeks to the specified amount of seconds
        /// </summary>
        /// <param name="position"></param>
        Task SeekTo(TimeSpan position);

        /// <summary>
        /// Enables or disables shuffling
        /// </summary>
        void ToggleShuffle();

        /// <summary>
        /// Enables or disables repeat mode
        /// </summary>
        void ToggleRepeat();

        event StateChangedEventHandler StateChanged;

        event PlayingChangedEventHandler PlayingChanged;

        event BufferingChangedEventHandler BufferingChanged;

        event PositionChangedEventHandler PositionChanged;

        event MediaItemFinishedEventHandler MediaItemFinished;

        event MediaItemChangedEventHandler MediaItemChanged;

        event MediaItemFailedEventHandler MediaItemFailed;
    }
}
