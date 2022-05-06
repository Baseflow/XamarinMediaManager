using Android.Runtime;
using Android.Support.V4.Media;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Source;
using MediaManager.Library;
using MediaManager.Platforms.Android.Media;

namespace MediaManager.Platforms.Android.Queue
{
    public class QueueDataAdapter : Java.Lang.Object, TimelineQueueEditor.IQueueDataAdapter
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;
        protected ConcatenatingMediaSource _mediaSource;

        public QueueDataAdapter(ConcatenatingMediaSource mediaSource)
        {
            _mediaSource = mediaSource;
            MediaManager.Queue.MediaItems.CollectionChanged += MediaQueue_CollectionChanged;
        }

        protected QueueDataAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public void Add(int index, MediaDescriptionCompat description)
        {
            MediaManager.Queue.Insert(index, description.ToMediaItem());
        }

        public MediaDescriptionCompat GetMediaDescription(int index)
        {
            return MediaManager.Queue.ElementAtOrDefault(index)?.ToMediaDescription();
        }

        public void Move(int oldIndex, int newIndex)
        {
            MediaManager.Queue.Move(oldIndex, newIndex);
        }

        public void Remove(int index)
        {
            MediaManager.Queue.RemoveAt(index);
        }

        private void MediaQueue_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (_mediaSource.Size != MediaManager.Queue.Count)
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
                            for (int i = 0; i < e.NewItems.Count; i++)
                                _mediaSource.MoveMediaSource(oldEndIndex, newBeginIndex);

                        //move when new is after old
                        else if (newBeginIndex > oldBeginIndex)
                            for (int i = 0; i < e.NewItems.Count; i++)
                                _mediaSource.MoveMediaSource(oldBeginIndex, newEndIndex);
                    }
                    else
                        _mediaSource.MoveMediaSource(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.OldItems.Count > 1)
                    {
                        for (int i = 0; i < e.OldItems.Count; i++)
                            _mediaSource.RemoveMediaSource(e.OldStartingIndex);
                    }
                    else
                        _mediaSource.RemoveMediaSource(e.OldStartingIndex);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    if (e.NewItems.Count > 1)
                    {
                        for (int i = 0; i < e.NewItems.Count; i++)
                            _mediaSource.RemoveMediaSource(e.OldStartingIndex);
                    }
                    else
                        _mediaSource.RemoveMediaSource(e.OldStartingIndex);

                    for (int i = e.NewItems.Count - 1; i >= 0; i--)
                    {
                        var mediaItem = (IMediaItem)e.NewItems[i];
                        _mediaSource.AddMediaSource(e.NewStartingIndex, mediaItem.ToMediaSource());
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    _mediaSource.Clear();
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            MediaManager.Queue.MediaItems.CollectionChanged -= MediaQueue_CollectionChanged;
            base.Dispose(disposing);
        }
    }
}
