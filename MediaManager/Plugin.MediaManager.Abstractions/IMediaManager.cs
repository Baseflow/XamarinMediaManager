using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager.Abstractions
{

    using System;

    public delegate void MediaFileChangedEventHandler(object sender, MediaFileChangedEventArgs e);

    public delegate void MediaFileFailedEventHandler(object sender, MediaFileFailedEventArgs e);

    /// <summary>
    /// The main purpose of this class is to be a controlling unit for all the single MediaItem implementations, who
    /// in themselve can play their media, but need a central controling unit, surrounding them
    /// </summary>
    /// <seealso cref="Plugin.MediaManager.Abstractions.IPlaybackManager" />
    public interface IMediaManager : IPlaybackManager
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
        IMediaNotificationManager MediaNotificationManager { get; set; }

        /// <summary>
        /// Extracts media information to put it into an IMediaFile
        /// </summary>
        IMediaExtractor MediaExtractor { get; set; }

        /// <summary>
        /// Raised when media is finished playing.
        /// </summary>
        event MediaFileChangedEventHandler MediaFileChanged;

        /// <summary>
        /// Raised when mediadata of MediaFile failed to update
        /// </summary>
        event MediaFileFailedEventHandler MediaFileFailed;
        
        /// <summary>
        /// Creates new MediaFile object, adds it to the queue and starts playing
        /// </summary>
        Task Play(string url, MediaFileType fileType);

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
        /// Adds all MediaFiles to the Queue and starts playing the first item
        /// </summary>
        Task Play(IEnumerable<IMediaFile> mediaFiles);

        /// <summary>
        /// Start playing if nothing is playing, otherwise it pauses the current media
        /// </summary>
        Task PlayPause();

        /// <summary>
        /// Sets a function which gets called before the track is played
        /// </summary>
        /// <param name="beforePlay">The before play.</param>
        void SetOnBeforePlay(Func<IMediaFile, Task> beforePlay);

    }
}
