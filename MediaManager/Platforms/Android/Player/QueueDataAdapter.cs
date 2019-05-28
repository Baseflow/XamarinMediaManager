using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Runtime;
using Android.Support.V4.Media;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Source;
using MediaManager.Media;

namespace MediaManager.Platforms.Android.Media
{
    public class QueueDataAdapter : Java.Lang.Object, TimelineQueueEditor.IQueueDataAdapter
    {
        private IMediaManager _mediaManager = CrossMediaManager.Current;
        private ConcatenatingMediaSource _mediaSource;

        public QueueDataAdapter(ConcatenatingMediaSource mediaSource)
        {
            _mediaSource = mediaSource;
            //_mediaManager.MediaQueue.CollectionChanged += MediaQueue_CollectionChanged;
        }

        protected QueueDataAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public void Add(int index, MediaDescriptionCompat description)
        {
            _mediaManager.MediaQueue.Insert(index, description.ToMediaItem());
        }

        public MediaDescriptionCompat GetMediaDescription(int index)
        {
            return _mediaManager.MediaQueue.ElementAtOrDefault(index)?.ToMediaDescription();
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
        //TODO: Find out if queue also need to get picked up on changes. Maybe when people add items directly to the queue while playing already.
        /*
        private void MediaQueue_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (_mediaSource.Size != _mediaManager.MediaQueue.Count)
                    {
                        for (int i = e.NewItems.Count - 1; i >= 0; i--)
                        {
                            var mediaItem = (IMediaItem)e.NewItems[i];
                            _mediaSource.AddMediaSource(e.NewStartingIndex, mediaItem.ToMediaSource());
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    if (e.NewItems.Count > 1)
                    {
                        int oldBeginIndex = e.OldStartingIndex;
                        int oldEndIndex = e.OldStartingIndex + e.NewItems.Count - 1;

                        int newBeginIndex = e.NewStartingIndex;
                        int newEndIndex = e.NewStartingIndex + e.NewItems.Count - 1;

                        //move when new is before old
                        if (newBeginIndex < oldBeginIndex)
                            for (int i = 0; i > e.NewItems.Count; i++)
                                _mediaSource.MoveMediaSource(oldEndIndex, newBeginIndex);

                        //move when new is after old
                        else if (newBeginIndex > oldBeginIndex)
                            for (int i = 0; i > e.NewItems.Count; i++)
                                _mediaSource.MoveMediaSource(oldBeginIndex, newEndIndex);
                    }
                    else
                        _mediaSource.MoveMediaSource(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.NewItems.Count > 1)
                    {
                        for (int i = 0; i > e.NewItems.Count; i++)
                            _mediaSource.RemoveMediaSource(e.OldStartingIndex);
                    }
                    else
                        _mediaSource.RemoveMediaSource(e.OldStartingIndex);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    throw new ArgumentException("Replacing in MediaQueue not supported.");
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    _mediaSource.Clear();
                    break;
            }
        }*/
    }
}
