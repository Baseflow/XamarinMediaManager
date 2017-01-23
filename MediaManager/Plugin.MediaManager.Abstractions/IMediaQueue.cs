using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager.Abstractions
{
    public delegate void QueueEndedEventHandler(object sender, QueueEndedEventArgs e);

    public delegate void QueueMediaChangedEventHandler(object sender, QueueMediaChangedEventArgs e);

    /// <summary>
    /// Manages all the items that will be played
    /// </summary>
    public interface IMediaQueue : IList<IMediaFile>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when the end of the Queue has been reached
        /// </summary>
        event QueueEndedEventHandler QueueEnded;

        /// <summary>
        /// Raised when the current Queue item changes
        /// </summary>
        event QueueMediaChangedEventHandler QueueMediaChanged;

        /// <summary>
        /// Get the current track from the Queue
        /// </summary>
        IMediaFile Current { get; }

        /// <summary>
        /// The type of repeat: None, RepeatOne or RepeatAll
        /// </summary>
        RepeatType Repeat { get; set; }

        /// <summary>
        /// Wether the queue is shuffled
        /// </summary>
        bool IsShuffled { get; }

        /// <summary>
        /// If the Queue has a next track
        /// </summary>
        bool HasNext();

        /// <summary>
        /// If the Queue has a previous track
        /// </summary>
        bool HasPrevious();

        /// <summary>
        /// Get the current playing index the Queue
        /// </summary>
        int Index { get; }

        void SetPreviousAsCurrent();

        void SetNextAsCurrent();

        void SetIndexAsCurrent(int index);

        void SetTrackAsCurrent(IMediaFile item);

        void AddRange(IEnumerable<IMediaFile> mediaFiles);

        /// <summary>
        /// Turns on or off shuffling.
        /// </summary>
        void ToggleShuffle();

        /// <summary>
        /// Toggles between the RepeatTypes: None, RepeatOne and RepeatAll
        /// </summary>
        void ToggleRepeat();
    }
}

