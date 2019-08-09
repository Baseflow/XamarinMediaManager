using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using MediaManager.Library;

namespace MediaManager.Queue
{
    public delegate void QueueEndedEventHandler(object sender, QueueEndedEventArgs e);

    public delegate void QueueChangedEventHandler(object sender, QueueChangedEventArgs e);

    public interface IMediaQueue : IList<IMediaItem>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when the end of the Queue has been reached
        /// </summary>
        event QueueEndedEventHandler QueueEnded;

        event QueueChangedEventHandler QueueChanged;

        /// <summary>
        /// If the Queue has a next track
        /// </summary>
        bool HasNext();

        /// <summary>
        /// Get the next item from the queue
        /// </summary>
        IMediaItem NextItem { get; }

        /// <summary>
        /// If the Queue has a previous track
        /// </summary>
        bool HasPrevious();

        /// <summary>
        /// Get the previous item from the queue
        /// </summary>
        IMediaItem PreviousItem { get; }


        /// <summary>
        /// If the Queue has a track it can currently play
        /// </summary>
        bool HasCurrent();

        /// <summary>
        /// Get the current playing index the Queue
        /// </summary>

        //TODO: Maybe rename to just Index
        int CurrentIndex { get; set; }

        /// <summary>
        /// Get the current track from the Queue
        /// </summary>
        IMediaItem Current { get; }

        string Title { get; set; }

        ShuffleMode ShuffleMode { get; set; }

        void Move(int oldIndex, int newIndex);
    }
}
