using System.Threading.Tasks;

namespace Plugin.MediaManager.Abstractions
{
    public interface IPlaybackController
    {
        /// <summary>
        /// Plays or pauses the currentl MediaFile
        /// </summary>
        Task PlayPause();

        /// <summary>
        /// Plays the current MediaFile
        /// </summary>
        Task Play();

        /// <summary>
        /// Pauses the current MediaFile
        /// </summary>
        Task Pause();

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

        /// <summary>
        /// Seeks to the start of the current MediaFile
        /// </summary>
        Task SeekToStart();

        /// <summary>
        /// Seeks forward a fixed amount of seconds of the current MediaFile
        /// </summary>
        Task StepForward();

        /// <summary>
        /// Seeks backward a fixed amount of seconds of the current MediaFile
        /// </summary>
        Task StepBackward();

        /// <summary>
        /// Toggles between the different repeat: modes None, RepeatOne and RepeatAll
        /// </summary>
        void ToggleRepeat();

        /// <summary>
        /// Enables or disables shuffling
        /// </summary>
        void ToggleShuffle();
    }
}