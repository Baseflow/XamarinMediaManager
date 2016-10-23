using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace Plugin.MediaManager.Abstractions.Implementations
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

        public bool Shuffle
        {
            get
            {
                return _unshuffledQueue != null;
            }
        }

        private bool IsRepeat
        {
            get
            {
                return repeat == RepeatType.RepeatAll || repeat == RepeatType.RepeatOne;
            }
        }

        private RepeatType repeat;
        public RepeatType Repeat
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

        public IMediaFile this[int index]
        {
            get
            {
                return _queue[index];
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                if (Shuffle)
                {
                    var unshuffledIndex = _unshuffledQueue.IndexOf(_queue[index]);
                    _unshuffledQueue[unshuffledIndex] = value;
                }

                _queue[index] = value;
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
        public event QueueEndedEventHandler QueueEnded;
        public event QueueMediaChangedEventHandler QueueMediaChanged;

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
            return Index + 1 < _queue.Count || IsRepeat;
        }

        public bool HasPrevious()
        {
            return Index - 1 >= 0 || IsRepeat;
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

        public void ToggleRepeat(RepeatType repeatType)
        {
            switch (repeatType)
            {
                case RepeatType.None:
                    Repeat = RepeatType.RepeatOne;
                    break;
                case RepeatType.RepeatOne:
                    Repeat = RepeatType.RepeatAll;
                    break;
                case RepeatType.RepeatAll:
                    Repeat = RepeatType.None;
                    break;
            }
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

        public int IndexOf(IMediaFile item)
        {
            return _queue.IndexOf(item);
        }

        public void Insert(int index, IMediaFile item)
        {
            if (Shuffle)
            {
                _unshuffledQueue.Add(item);
            }

            var changedIndexInternally = false;
            if (index <= Index || Index == -1)
            {
                _index++;
                changedIndexInternally = true;
            }

            try
            {
                _queue.Insert(index, item);
            }
            catch (Exception)
            {
                if (changedIndexInternally)
                {
                    _index--;
                }
                throw;
            }

            // Only to fire off the index when also the property Current is updated (by triggering RemoveAt())
            if (changedIndexInternally)
            {
                OnPropertyChanged(nameof(Index));
            }
        }

        public void RemoveAt(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            // If shuffle is enabled, we need to remove the item from the backup queue too
            if (Shuffle)
            {
                _unshuffledQueue.Remove(_queue[index]);
            }

            var changedIndexInternally = false;
            if (index < Index)
            {
                _index--;
                changedIndexInternally = true;
            }
            else if (index == Index && Index == Count - 1)
            {
                _index = index - 1;
                changedIndexInternally = true;
            }

            _queue.RemoveAt(index);

            // Only to fire off the index when also the property Current is updated (by triggering RemoveAt())
            if (changedIndexInternally)
            {
                OnPropertyChanged(nameof(Index));
            }
        }
    }
}

