using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MediaManager.Media
{
    public delegate void QueueEndedEventHandler(object sender, QueueEndedEventArgs e);

    public delegate void QueueMediaChangedEventHandler(object sender, QueueMediaChangedEventArgs e);

    public interface IMediaQueue : IList<IMediaItem>, INotifyCollectionChanged, INotifyPropertyChanged
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

        /// <summary>
        /// Get the current track from the Queue
        /// </summary>
        IMediaItem Current { get; }
    }
}
