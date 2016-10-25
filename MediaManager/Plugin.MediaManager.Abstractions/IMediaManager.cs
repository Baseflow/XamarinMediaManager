using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager.Abstractions
{
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);

    public delegate void PlayingChangedEventHandler(object sender, PlayingChangedEventArgs e);

    public delegate void BufferingChangedEventHandler(object sender, BufferingChangedEventArgs e);

    public delegate void MediaFinishedEventHandler(object sender, MediaFinishedEventArgs e);

    public delegate void MediaFailedEventHandler(object sender, MediaFailedEventArgs e);

    public delegate void MediaFileChangedEventHandler(object sender, MediaFileChangedEventArgs e);

    public delegate void MediaFileFailedEventHandler(object sender, MediaFileFailedEventArgs e);

    /// <summary>
    /// The main purpose of this class is to be a controlling unit for all the single MediaItem implementations, who
    /// in themselve can play their media, but need a central controling unit, surrounding them
    /// </summary>
    public interface IMediaManager : IPlaybackManager
    {
        /// <summary>
        /// Player responseble for audio playback
        /// </summary>
        IAudioPlayer AudioPlayer { get; }

        /// <summary>
        /// Player responseble for video playback
        /// </summary>
        IVideoPlayer VideoPlayer { get; }

        /// <summary>
        /// Queue to play media in sequences
        /// </summary>
        IMediaQueue MediaQueue { get; }

        /// <summary>
        /// Manages notifications to the native system
        /// </summary>
        IMediaNotificationManager  MediaNotificationManager { get; }

        /// <summary>
        /// Extracts media information to put it into an IMediaFile
        /// </summary>
        IMediaExtractor MediaExtractor { get; }

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
    }
}
