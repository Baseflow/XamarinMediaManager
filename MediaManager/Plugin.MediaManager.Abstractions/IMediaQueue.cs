using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Plugin.MediaManager.Abstractions
{
    public interface IMediaQueue : ICollection<IMediaFile>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// Activates or deactivates the Repeat option
        /// </summary>
        bool Repeat { get; }

        /// <summary>
        /// Activates or deactivates the Shuffle option
        /// </summary>
        bool Shuffle { get; }

        /// <summary>
        /// If the Queue has a next track
        /// </summary>
        bool HasNext();

        /// <summary>
        /// If the Queue has a previous track
        /// </summary>
        bool HasPrevious();

        /// <summary>
        /// Get the current track from the Queue
        /// </summary>
        IMediaFile Current { get; }

        /// <summary>
        /// Get the current playing index the Queue
        /// </summary>
        int Index { get; }

        void SetPreviousAsCurrent();

        void SetNextAsCurrent();

        void SetIndexAsCurrent(int index);

        void SetTrackAsCurrent(IMediaFile item);

        void AddRange(IEnumerable<IMediaFile> items);

        /// <summary>
        /// Turns on or off shuffling.
        /// </summary>
        void ToggleShuffle();

        /// <summary>
        /// Turns on or off repeat.
        /// </summary>
        void ToggleRepeat();
    }
}

