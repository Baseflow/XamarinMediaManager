using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AVFoundation;
using MediaManager.Media;

namespace MediaManager.Platforms.Apple.Media
{
    public class QueueDataAdapter
    {
        private IMediaManager _mediaManager = CrossMediaManager.Current;
        private AVQueuePlayer _avQueuePlayer => ((AVQueuePlayer)_mediaManager.MediaPlayer);

        public QueueDataAdapter()
        {
            _mediaManager.MediaQueue.CollectionChanged += MediaQueue_CollectionChanged;
        }

        public void Add(int index, IMediaItem mediaItem)
        {
            _mediaManager.MediaQueue.Insert(index, mediaItem);
        }

        public void Move(int oldIndex, int newIndex)
        {
            if (_mediaManager.MediaQueue is ObservableCollection<IMediaItem> observableCollection)
                observableCollection.Move(oldIndex, newIndex);
        }

        public void Remove(int index)
        {
            _mediaManager.MediaQueue.RemoveAt(index);
        }

        private void MediaQueue_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (_avQueuePlayer.Items.Count() != CrossMediaManager.Current.MediaQueue.Count)
                    {
                        for (int i = e.NewItems.Count - 1; i >= 0; i--)
                        {
                            var mediaItem = (IMediaItem)e.NewItems[i];
                            _avQueuePlayer.InsertItem(mediaItem.GetPlayerItem(), null);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    throw new ArgumentException("Moving in the apple queue is not yet supported");
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.NewItems.Count > 1)
                    {
                        for (int i = 0; i > e.NewItems.Count; i++)
                            _avQueuePlayer.RemoveItem(_avQueuePlayer.Items[e.OldStartingIndex]);
                    }
                    else
                        _avQueuePlayer.RemoveItem(_avQueuePlayer.Items[e.OldStartingIndex]);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    throw new ArgumentException("Replacing in MediaQueue not supported.");
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    _avQueuePlayer.RemoveAllItems();
                    break;
            }
        }
    }
}
