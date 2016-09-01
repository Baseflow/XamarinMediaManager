using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace Plugin.MediaManager.Abstractions
{
    public class MediaQueue : IMediaQueue
    {
        private ObservableCollection<IMediaFile> _queue;

        private ObservableCollection<IMediaFile> _unshuffledQueue;

        public int Count
        {
            get
            {
                return _unshuffledQueue?.Count ?? _queue?.Count ?? 0;
            }
        }

        private IMediaFile _current;
        public IMediaFile Current
        {
            get
            {
                if (Count > 0 && Index >= 0)
                {
                    return _current = _queue[Index];
                }

                return _current = null;
            }
        }

        private int _index = -1;
        public int Index
        {
            get
            {
                return _index;
            }
            private set
            {
                _index = value;
                OnPropertyChanged(nameof(Index));
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        private bool repeat;
        public bool Repeat
        {
            get
            {
                return repeat;
            }
            private set
            {
                repeat = value;
                OnPropertyChanged(nameof(Repeat));
            }
        }

        public bool Shuffle
        {
            get
            {
                return _unshuffledQueue != null;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public void Add(IMediaFile item)
        {
            _queue.Add(item);

            // If shuffle is enabled, we need to add the item to the backup queue too
            if (Shuffle)
            {
                _unshuffledQueue.Add(item);
            }

            if (Index == -1)
            {
                Index = 0;
            }
        }

        public void AddRange(IEnumerable<IMediaFile> items)
        {
            _queue.AddRange(items);

            // If shuffle is enabled, we need to add the items to the backup queue too
            if (Shuffle)
            {
                _unshuffledQueue.AddRange(items);
            }

            if (Index == -1)
            {
                Index = 0;
            }
        }

        public void Clear()
        {
            Index = -1;
            _queue.Clear();
            _unshuffledQueue = null;
        }

        public bool Contains(IMediaFile item)
        {
            return _queue.Contains(item);
        }

        public void CopyTo(IMediaFile[] array, int arrayIndex)
        {
            _queue.CopyTo(array, arrayIndex);

            if (Index == -1)
            {
                Index = 0;
            }
        }

        public IEnumerator<IMediaFile> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        public bool HasNext()
        {
            return Index + 1 < _queue.Count || Repeat;
        }

        public bool HasPrevious()
        {
            return Index - 1 >= 0 || Repeat;
        }

        public bool Remove(IMediaFile item)
        {
            var result = _queue.Remove(item);

            // If shuffle is enabled, we need to remove the item from the backup queue too
            if (result && Shuffle)
            {
                _unshuffledQueue.Remove(item);
            }

            return result;
        }

        public void SetIndexAsCurrent(int index)
        {
            Index = index;
        }

        public void SetPreviousAsCurrent()
        {
            if (Index - 1 >= 0)
            {
                Index--;
            }
            else
            {
                Index = _queue.Count - 1;
            }
        }

        public void SetNextAsCurrent()
        {
            if (Index + 1 < _queue.Count)
            {
                Index++;
            }
            else
            {
                Index = 0;
            }
        }

        public void SetTrackAsCurrent(IMediaFile item)
        {
            if (_queue.Contains(item))
                Index = _queue.IndexOf(item);
        }

        public void ToggleRepeat()
        {
            Repeat = !Repeat;
        }

        private CancellationTokenSource shuffleCancellation = new CancellationTokenSource();
        public void ToggleShuffle()
        {
            if (!Shuffle)
            {
                // Cancel any running tasks
                shuffleCancellation.Cancel();

                // Set up new cancellation token so we can run this task
                var cts = new CancellationTokenSource();
                shuffleCancellation = cts;

                // Work with a copy of the current queue
                var elements = _queue.ToList();

                // Remove the current item to add it as first item again later
                var currentItem = Current;
                if (currentItem != null)
                {
                    elements.RemoveAt(Index);
                }

                for (var i = elements.Count - 1; i > 1; i--)
                {
                    // Get a random index
                    //var swapIndex = Math.Abs((int)WinRTCrypto.CryptographicBuffer.GenerateRandomNumber()) % (i + 1);

                    var random = new Random((int)DateTime.Now.Ticks);
                    var swapIndex = random.Next(0, i + 1);

                    var tmp = elements[i];
                    elements[i] = elements[swapIndex];
                    elements[swapIndex] = tmp;
                }

                // If the cancellation token canceled in the meantime, exit here
                if (cts.Token.IsCancellationRequested)
                {
                    return;
                }

                // Back up current queue to backup queue so we can get it later when user turns off shuffling
                _unshuffledQueue = new ObservableCollection<IMediaFile>(_queue);

                if (currentItem != null)
                {
                    // Add currentItem to queue as first index
                    elements.Insert(0, currentItem);
                }

                // Replace queue with randomized collection
                _queue.Clear();
                _queue.AddRange(elements);

                Index = 0;
            }
            else
            {
                // Reset queues
                var newIndex = _unshuffledQueue.IndexOf(Current);
                _queue.Clear();
                _queue.AddRange(_unshuffledQueue);
                Index = newIndex;
                _unshuffledQueue = null;
            }
            OnPropertyChanged(nameof(Shuffle));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }
    }
}

