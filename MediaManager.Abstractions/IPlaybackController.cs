using System;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager.Abstractions
{
    public interface IPlaybackController
    {
        /// <summary>
        /// Plays the current MediaItem
        /// </summary>
        Task Play();

        /// <summary>
        /// Pauses the current MediaItem
        /// </summary>
        Task Pause();

        /// <summary>
        /// Plays or pauses the current MediaItem
        /// </summary>
        Task PlayPause();

        /// <summary>
        /// Stops playing
        /// </summary>
        Task Stop();

        /// <summary>
        /// Plays the previous MediaItem or seeks to start if far enough into the current one.
        /// </summary>
        Task PlayPreviousOrSeekToStart();

        /// <summary>
        /// Plays the previous MediaItem
        /// </summary>
        Task PlayPrevious();

        /// <summary>
        /// Plays the next MediaItem
        /// </summary>
        /// <returns></returns>
        Task PlayNext();

        Task PlayFromQueueByIndex(int index);

        Task PlayFromQueueByMediaItem(IMediaItem file);

        /// <summary>
        /// Seeks to the start of the current MediaItem
        /// </summary>
        Task SeekToStart();

        /// <summary>
        /// Seeks forward a fixed amount of time of the current MediaItem
        /// </summary>
        /// <param name="time"></param>
        Task SeekForward(TimeSpan? time = null);

        /// <summary>
        /// Seeks backward a fixed amount of time of the current MediaItem
        /// </summary>
        /// <param name="time"></param>
        Task SeekBackward(TimeSpan? time = null);

        /// <summary>
        /// Seeks to the specified amount of time
        /// </summary>
        /// <param name="position"></param>
        Task SeekTo(TimeSpan position);

        /// <summary>
        /// Toggles between the different repeat: modes None, RepeatOne and RepeatAll
        /// </summary>
        void SetRepeatMode(RepeatMode type);

        /// <summary>
        /// Enables or disables shuffling
        /// </summary>
        void SetShuffleMode(ShuffleMode type);

        //TODO: add rating implementation
        void SetRating();
    }
}