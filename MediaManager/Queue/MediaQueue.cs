using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager.Queue
{
    public class MediaQueue : ObservableCollection<IMediaItem>, IMediaQueue
    {
        IMediaManager MediaManager = CrossMediaManager.Current;

        public event QueueEndedEventHandler QueueEnded;

        public event QueueChangedEventHandler QueueChanged;

        public bool HasNext() => Count > CurrentIndex - 1;

        public IMediaItem NextItem
        {
            get
            {
                if (HasNext())
                {
                    CurrentIndex++;
                    return Current;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool HasPrevious() => CurrentIndex > 0;
        public IMediaItem PreviousItem
        {
            get
            {
                if (HasPrevious())
                {
                    CurrentIndex--;
                    return Current;
                } else
                {
                    return null;
                }
            }
        }


        public bool HasCurrent() => Count >= CurrentIndex;
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

        public void Shuffle()
        {
            Random rng = new Random();
            var rndItems = this.OrderBy(a => rng.Next());
            this.Clear();
            foreach (var rndItem in rndItems)
            {
                this.Add(rndItem);
            }
        }

        public string Title { get; set; }

        internal void OnQueueEnded(object s, QueueEndedEventArgs e) => QueueEnded?.Invoke(s, e);

        internal void OnQueueChanged(object s, QueueChangedEventArgs e) => QueueChanged?.Invoke(s, e);
    }
}
