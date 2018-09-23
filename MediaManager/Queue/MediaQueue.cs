using System;
using System.Collections.ObjectModel;
using MediaManager.Media;

namespace MediaManager.Queue
{
    public class MediaQueue : ObservableCollection<IMediaItem>, IMediaQueue
    {
        public event QueueEndedEventHandler QueueEnded;

        public event QueueChangedEventHandler QueueChanged;

        public bool HasNext()
        {
            return Count > CurrentIndex - 1;
        }

        public bool HasPrevious()
        {
            return CurrentIndex > 0;
        }

        public IMediaItem Current => Count > 0 ? this[CurrentIndex] : null;

        private int _currentIndex = 0;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (_currentIndex != value)
                    OnQueueChanged(this, new QueueChangedEventArgs());
                _currentIndex = value;
            }
        }

        public string Title { get; set; }

        internal void OnQueueEnded(object s, QueueEndedEventArgs e) => QueueEnded?.Invoke(s, e);

        internal void OnQueueChanged(object s, QueueChangedEventArgs e) => QueueChanged?.Invoke(s, e);
    }
}
