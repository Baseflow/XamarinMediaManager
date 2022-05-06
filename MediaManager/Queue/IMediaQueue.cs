using System.Collections.ObjectModel;
using System.ComponentModel;
using MediaManager.Library;

namespace MediaManager.Queue
{
    public delegate void QueueEndedEventHandler(object sender, QueueEndedEventArgs e);

    public delegate void QueueChangedEventHandler(object sender, QueueChangedEventArgs e);

    public interface IMediaQueue : IList<IMediaItem>, INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when the end of the Queue has been reached
        /// </summary>
        event QueueEndedEventHandler QueueEnded;

        event QueueChangedEventHandler QueueChanged;

        ObservableCollection<IMediaItem> MediaItems { get; }

        string Title { get; set; }

        /// <summary>
        /// If the Queue has a next track
        /// </summary>
        bool HasNext { get; }

        /// <summary>
        /// Get the next item from the queue
        /// </summary>
        IMediaItem Next { get; }

        /// <summary>
        /// If the Queue has a previous track
        /// </summary>
        bool HasPrevious { get; }

        /// <summary>
        /// Get the previous item from the queue
        /// </summary>
        IMediaItem Previous { get; }

        /// <summary>
        /// If the Queue has a track it can currently play
        /// </summary>
        bool HasCurrent { get; }

        /// <summary>
        /// Get the current playing index the Queue
        /// </summary>
        int CurrentIndex { get; set; }

        /// <summary>
        /// Get the current track from the Queue
        /// </summary>
        IMediaItem Current { get; }

        void Move(int oldIndex, int newIndex);
    }
}
