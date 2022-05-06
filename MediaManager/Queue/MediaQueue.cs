using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using MediaManager.Library;
using MediaManager.Media;

namespace MediaManager.Queue
{
    public class MediaQueue : NotifyPropertyChangedBase, IMediaQueue
    {
        protected MediaManagerBase MediaManager => CrossMediaManager.Current as MediaManagerBase;

        public MediaQueue()
        {
            MediaItems.CollectionChanged += MediaItems_CollectionChanged;
            MediaManager.PropertyChanged += MediaManager_PropertyChanged;
            MediaManager.MediaItemFinished += MediaManager_MediaItemFinished;
        }

        private int shuffleKey = int.MinValue;
        private void MediaManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MediaManager.ShuffleMode))
            {
                if (MediaManager.ShuffleMode == ShuffleMode.All)
                {
                    shuffleKey = new Random().Next(int.MinValue + 1, int.MaxValue);
                    MediaItems.Shuffle(shuffleKey);
                }
                else if (shuffleKey != int.MinValue)
                {
                    MediaItems.DeShuffle(shuffleKey);
                    shuffleKey = int.MinValue;
                }
            }
        }

        private void MediaItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnQueueChanged(this, new QueueChangedEventArgs(Current));
        }

        public event QueueEndedEventHandler QueueEnded;

        public event QueueChangedEventHandler QueueChanged;

        public ObservableCollection<IMediaItem> MediaItems { get; protected set; } = new ObservableCollection<IMediaItem>();

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public bool HasNext => MediaItems.Count > CurrentIndex + 1;

        public IMediaItem Next
        {
            get
            {
                if (HasNext)
                {
                    return this[CurrentIndex + 1];
                }
                else
                {
                    return null;
                }
            }
        }

        public bool HasPrevious => CurrentIndex > 0;

        public IMediaItem Previous
        {
            get
            {
                if (HasPrevious)
                {
                    return this[CurrentIndex - 1];
                }
                else
                {
                    return null;
                }
            }
        }

        public bool HasCurrent => this.ElementAtOrDefault(CurrentIndex) != null;

        public IMediaItem Current
        {
            get => this.ElementAtOrDefault(CurrentIndex);
            internal set => CurrentIndex = MediaItems.IndexOf(value);
        }

        private int _currentIndex = 0;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                SetProperty(ref _currentIndex, value);
                if (Current != null)
                {
                    OnQueueChanged(this, new QueueChangedEventArgs(Current));
                    MediaManager.OnMediaItemChanged(this, new MediaItemEventArgs(Current));
                }
            }
        }

        internal void OnQueueEnded(object s, QueueEndedEventArgs e) => QueueEnded?.Invoke(s, e);

        internal void OnQueueChanged(object s, QueueChangedEventArgs e)
        {
            QueueChanged?.Invoke(s, e);
            MediaManager?.Notification?.UpdateNotification();
        }

        private void MediaManager_MediaItemFinished(object sender, MediaItemEventArgs e)
        {
            if (MediaItems == null || MediaItems.Count == 0) return;

            if (MediaItems.Last() == e.MediaItem)
                OnQueueEnded(this, new QueueEndedEventArgs(e.MediaItem));
        }

        public int Count => MediaItems.Count;

        public bool IsReadOnly => false;

        public IMediaItem this[int index] { get => MediaItems[index]; set => MediaItems[index] = value; }

        public IEnumerator<IMediaItem> GetEnumerator()
        {
            return MediaItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return MediaItems.GetEnumerator();
        }

        public int IndexOf(IMediaItem item)
        {
            return MediaItems.IndexOf(item);
        }

        public void Insert(int index, IMediaItem item)
        {
            MediaItems.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            MediaItems.RemoveAt(index);
        }

        public void Add(IMediaItem item)
        {
            MediaItems.Add(item);
        }

        public void Clear()
        {
            MediaItems.Clear();
        }

        public bool Contains(IMediaItem item)
        {
            return MediaItems.Contains(item);
        }

        public void CopyTo(IMediaItem[] array, int arrayIndex)
        {
            MediaItems.CopyTo(array, arrayIndex);
        }

        public bool Remove(IMediaItem item)
        {
            return MediaItems.Remove(item);
        }

        public void Move(int oldIndex, int newIndex)
        {
            MediaItems.Move(oldIndex, newIndex);
        }
    }
}
