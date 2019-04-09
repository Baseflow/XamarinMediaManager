using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Playback;

namespace MediaManager
{
    public delegate void BeforePlayingEventHandler(object sender, MediaPlayerEventArgs e);

    public delegate void AfterPlayingEventHandler(object sender, MediaPlayerEventArgs e);

    public interface IMediaPlayer<TPlayer> : IMediaPlayer where TPlayer : class
    {
        TPlayer Player { get; set; }
    }

    public interface IMediaPlayer : IDisposable
    {
        void Initialize();

        /// <summary>
        /// Adds MediaFile to the Queue and starts playing
        /// </summary>
        Task Play(IMediaItem mediaItem);

        /// <summary>
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
        MediaPlayerState State { get; }

        /// <summary>
        /// Setting or getting whether we are in the repeat state
        /// </summary>
        RepeatMode RepeatMode { get; set; }

        event BeforePlayingEventHandler BeforePlaying;

        event AfterPlayingEventHandler AfterPlaying;
    }
}
