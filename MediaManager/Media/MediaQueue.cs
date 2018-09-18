using System;
using System.Collections.ObjectModel;

namespace MediaManager.Media
{
    public class MediaQueue : ObservableCollection<IMediaItem>, IMediaQueue
    {
        public event QueueEndedEventHandler QueueEnded;

        public bool HasNext()
        {
            return Count > Index - 1;
        }

        public bool HasPrevious()
        {
            return Index > 0;
        }

        public IMediaItem Current => this[Index];

        public int Index => currentIndex;

        internal void OnQueueEnded(object s, QueueEndedEventArgs e) => QueueEnded?.Invoke(s, e);

        internal int currentIndex = -1;
    }
}
