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
        public MediaQueue ()
        {
            _queue = new ObservableCollection<IMediaFile>();

            _queue.CollectionChanged += (sender, e) =>
            {
                if (CollectionChanged != null && !CollectionChangedEventDisabled)
                    CollectionChanged(sender, e);
            };

            RegisterCountTriggers();
            RegisterCurrentTriggers();
        }

        protected ObservableCollection<IMediaFile> _queue { get; private set; }

        protected ObservableCollection<IMediaFile> _unshuffledQueue;

        private int _count;
        public int Count
        {
            get
            {
                return _count;
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

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private bool CollectionChangedEventDisabled;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
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

            if (_unshuffledQueue != null)
            {
                _unshuffledQueue = null;
                OnPropertyChanged(nameof(Shuffle));
            }
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
            int index = _queue.IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
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

                var random = new Random();
                for (var i = elements.Count - 1; i > 1; i--)
                {
                    // Get a random index
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
                ReplaceQueueWith(elements);

                Index = 0;
            }
            else
            {
                // Reset queues
                var newIndex = _unshuffledQueue.IndexOf(Current);
                ReplaceQueueWith(_unshuffledQueue);
                Index = newIndex;
                _unshuffledQueue = null;
            }
            OnPropertyChanged(nameof(Shuffle));
        }

        protected void ReplaceQueueWith(IEnumerable<IMediaFile> files)
        {
            CollectionChangedEventDisabled = true;
            _queue.Clear();
            _queue.AddRange(files);
            CollectionChangedEventDisabled = false;
            if (CollectionChanged != null)
                CollectionChanged(_queue, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void RegisterCountTriggers()
        {
            _queue.CollectionChanged += (sender, e) =>
            {
                var count = _queue.Count;

                if (_count != count)
                {
                    _count = count;
                    OnPropertyChanged(nameof(Count));
                }
            };

            _count = _queue.Count;
        }

        private void RegisterCurrentTriggers()
        {
            var updateProperty = new Action(() =>
                {
                    IMediaFile current = null;
                    if (Count - 1 >= Index && Index >= 0)
                    {
                        current = _queue[Index];
                    }

                    if (_current != current)
                    {
                        _current = current;
                        OnPropertyChanged(nameof(Current));
                    }
                });

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Index))
                {
                    updateProperty();
                }
            };

            _queue.CollectionChanged += (sender, e) =>
            {
                updateProperty();
            };

            if (Count - 1 >= Index && Index >= 0)
            {
                _current = _queue[Index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }
    }
}

