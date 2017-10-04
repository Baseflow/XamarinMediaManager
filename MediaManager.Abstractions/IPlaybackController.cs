using System;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager.Abstractions
{
    public interface IPlaybackController
    {
        /// <summary>
        /// Plays the current MediaFile
        /// </summary>
        Task Play();

        /// <summary>
        /// Pauses the current MediaFile
        /// </summary>
        Task Pause();

        /// <summary>
        /// Plays or pauses the current MediaFile
        /// </summary>
        Task PlayPause();

        /// <summary>
        /// Stops playing
        /// </summary>
        Task Stop();

        /// <summary>
        /// Plays the previous MediaFile or seeks to start if far enough into the current one.
        /// </summary>
        Task PlayPreviousOrSeekToStart();

        /// <summary>
        /// Plays the previous MediaFile
        /// </summary>
        Task PlayPrevious();

        /// <summary>
        /// Plays the next MediaFile
        /// </summary>
        /// <returns></returns>
        Task PlayNext();

        Task PlayFromQueueByIndex(int index);

        Task PlayFromQueueByMediaFile(IMediaItem file);

        /// <summary>
        /// Seeks to the start of the current MediaFile
        /// </summary>
        Task SeekToStart();

        /// <summary>
        /// Seeks forward a fixed amount of seconds of the current MediaFile
        /// </summary>
        Task SeekForward(TimeSpan? time = null);

        /// <summary>
        /// Seeks backward a fixed amount of seconds of the current MediaFile
        /// </summary>
        Task SeekBackward(TimeSpan? time = null);

        /// <summary>
        /// Seeks to the specified amount of seconds
        /// </summary>
        /// <param name="seconds"></param>
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