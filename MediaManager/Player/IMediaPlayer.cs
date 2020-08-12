using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediaManager.Library;
using MediaManager.Video;

namespace MediaManager.Player
{
    public delegate void BeforePlayingEventHandler(object sender, MediaPlayerEventArgs e);

    public delegate void AfterPlayingEventHandler(object sender, MediaPlayerEventArgs e);

    public interface IMediaPlayer<TPlayer, TPlayerView> : IMediaPlayer<TPlayer> where TPlayer : class where TPlayerView : class, IVideoView
    {
        TPlayerView PlayerView { get; }
    }

    public interface IMediaPlayer<TPlayer> : IMediaPlayer where TPlayer : class
    {
        TPlayer Player { get; set; }
    }

    public interface IMediaPlayer : IDisposable, INotifyPropertyChanged
    {

        //TODO: Maybe introduce a source property to find the current playing item
        //IMediaItem Source { get; internal set; }

        IVideoView VideoView { get; set; }

        //TODO: See if we can make this cross platform
        //object PlaceholderImage { get; set; }

        bool AutoAttachVideoView { get; set; }

        VideoAspectMode VideoAspect { get; set; }

        bool ShowPlaybackControls { get; set; }

        int VideoWidth { get; }

        int VideoHeight { get; }

        float VideoAspectRatio { get; }

        object VideoPlaceholder { get; set; }

        /// <summary>
        /// Starts playing the MediaItem
        /// </summary>
        Task Play(IMediaItem mediaItem);

        /// <summary>
        /// Starts playing the MediaItem at a given time and stops at a specific time.
        /// Use TimeSpan.Zero for startAt to start at beginning of the MediaItem
        /// </summary>
        Task Play(IMediaItem mediaItem, TimeSpan startAt, TimeSpan? stopAt = null);

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
        Task SeekTo(TimeSpan position);

        /// <summary>
        /// Setting or getting whether we are in the repeat state
        /// </summary>
        //RepeatMode RepeatMode { get; set; }

        event BeforePlayingEventHandler BeforePlaying;

        event AfterPlayingEventHandler AfterPlaying;
    }
}
