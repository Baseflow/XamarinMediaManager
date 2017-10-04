using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager.Abstractions.Implementations
{
    public class MediaQueue : IMediaQueue
    {
        public MediaQueue ()
        {
            _queue = new ObservableCollection<IMediaItem>();

            _queue.CollectionChanged += (sender, e) =>
            {
                if (CollectionChanged != null && !CollectionChangedEventDisabled)
                    CollectionChanged(sender, e);
            };

            RegisterCountTriggers();
            RegisterCurrentTriggers();
        }

        protected ObservableCollection<IMediaItem> _queue { get; private set; }

        protected ObservableCollection<IMediaItem> _unshuffledQueue;

        private int _count;
        public int Count
        {
            get
            {
                return _count;
            }
        }

        private IMediaItem _current;
        public IMediaItem Current
        {
            get
            {
                if (Count > 0 && Index >= 0)
                {
                    return _queue[Index];
                }

                return null;
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

        public bool IsShuffled
        {
            get
            {
                return _unshuffledQueue != null;
            }
            set
            {
                var shuffled = value;
                if (shuffled != IsShuffled)
                {
                    if (shuffled)
                    {
                        Shuffle();
                    }
                    else
                    {
                        Unshuffle();
                    }

                    if (CollectionChanged != null)
                        CollectionChanged(_queue, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                    OnPropertyChanged(nameof(IsShuffled));
                }
            }
        }

        private bool IsRepeat
        {
            get
            {
                return repeat == RepeatMode.RepeatAll || repeat == RepeatMode.RepeatOne;
            }
        }

        private RepeatMode repeat = RepeatMode.None;
        public RepeatMode Repeat
        {
            get
            {
                return repeat;
            }
            set
            {
                repeat = value;
                OnPropertyChanged(nameof(Repeat));
            }
        }

        public IMediaItem this[int index]
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

                if (IsShuffled)
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
        public event QueueEndedEventHandler Ended;
        public event QueueMediaChangedEventHandler MediaItemChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Add(IMediaItem item)
        {
            _queue.Add(item);

            // If shuffle is enabled, we need to add the item to the backup queue too
            if (IsShuffled)
            {
                _unshuffledQueue.Add(item);
            }

            if (Index == -1)
            {
                Index = 0;
            }
        }

        public void AddRange(IEnumerable<IMediaItem> mediaFiles)
        {
            mediaFiles = mediaFiles.ToList();

            ExecuteWithoutCollectionChangedEvents(() => _queue.AddRange(mediaFiles));

            if (CollectionChanged != null)
            {
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, mediaFiles);

                CollectionChanged(_queue, eventArgs);
            }

            // If shuffle is enabled, we need to add the items to the backup queue too
            if (IsShuffled)
            {
                _unshuffledQueue.AddRange(mediaFiles);
            }

            if (Index == -1)
            {
                Index = 0;
            }
        }

        private void ExecuteWithoutCollectionChangedEvents(Action action)
        {
            bool disabledExplicitly = false;

            if (!CollectionChangedEventDisabled)
            {
                disabledExplicitly = true;
                CollectionChangedEventDisabled = true;
            }

            action();

            if (disabledExplicitly)
            {
                CollectionChangedEventDisabled = false;
            }
        }

        public void Clear()
        {
            Index = -1;
            _queue.Clear();

            if (_unshuffledQueue != null)
            {
                _unshuffledQueue = null;
                OnPropertyChanged(nameof(IsShuffled));
            }
        }

        public bool Contains(IMediaItem item)
        {
            return _queue.Contains(item);
        }

        public void CopyTo(IMediaItem[] array, int arrayIndex)
        {
            _queue.CopyTo(array, arrayIndex);

            if (Index == -1)
            {
                Index = 0;
            }
        }

        public IEnumerator<IMediaItem> GetEnumerator()
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

        public bool Remove(IMediaItem item)
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

        public void SetTrackAsCurrent(IMediaItem item)
        {
            if (_queue.Contains(item))
                Index = _queue.IndexOf(item);
        }

        private CancellationTokenSource shuffleCancellation = new CancellationTokenSource();

        protected void ReplaceQueueWith(IEnumerable<IMediaItem> files)
        {
            ExecuteWithoutCollectionChangedEvents(() =>
            {
                _queue.Clear();
                _queue.AddRange(files);
            });
        }

        private void Shuffle()
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
            _unshuffledQueue = new ObservableCollection<IMediaItem>(_queue);

            if (currentItem != null)
            {
                // Add currentItem to queue as first index
                elements.Insert(0, currentItem);
            }
            Index = 0;
            // Replace queue with randomized collection
            ReplaceQueueWith(elements);
        }

        private void Unshuffle()
        {
            // Reset queues
            var newIndex = _unshuffledQueue.IndexOf(Current);
            ReplaceQueueWith(_unshuffledQueue);
            Index = newIndex;
            _unshuffledQueue = null;
        }

        private void RegisterCountTriggers()
        {
            CollectionChanged += (sender, e) =>
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
                    if (_queue?.LastOrDefault() == Current)
                        Ended?.Invoke(this, new QueueEndedEventArgs());
                
                    IMediaItem current = null;
                    if (Count - 1 >= Index && Index >= 0)
                    {
                        current = _queue[Index];
                    }

                    if (_current != current)
                    {
                        _current = current;
                        OnPropertyChanged(nameof(Current));
                        MediaItemChanged?.Invoke(this, new QueueMediaChangedEventArgs(Current));
                    }
                });

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Index))
                {
                    updateProperty();
                }
            };

            CollectionChanged += (sender, e) =>
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

        public int IndexOf(IMediaItem item)
        {
            return _queue.IndexOf(item);
        }

        public void Insert(int index, IMediaItem item)
        {
            if (IsShuffled)
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
            if (IsShuffled)
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

