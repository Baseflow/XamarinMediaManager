using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager
{
    public interface IMediaPlayer
    {
        // <summary>
        /// Adds MediaFile to the Queue and starts playing
        /// </summary>
        Task Play(string Url);

        // <summary>
        /// Starts playing
        /// </summary>
        Task Play();

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
        /// Reading the current status of the player
        /// </summary>
        MediaPlayerStatus Status { get; }
    }
}
