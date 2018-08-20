using System;
using System.Collections.Generic;
using System.Linq;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using MediaManager.Media;

namespace MediaManager
{
    public class AndroidMediaQueue : MediaQueue
    {
        protected IList<MediaSessionCompat.QueueItem> AndroidQueue { private get; set; }

        public new void Add(IMediaItem item)
        {
            base.Add(item);

            AndroidQueue.Add(GetQueueItem(item));
        }

        public new void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }

        protected override void SetItem(int index, IMediaItem item)
        {
            base.SetItem(index, item);

            AndroidQueue[index] = GetQueueItem(item);
        }

        public new void Remove(IMediaItem item)
        {
            RemoveItem(IndexOf(item));
        }

        public new void RemoveAt(int index)
        {
            RemoveItem(index);
        }

        public new void Insert(int index, IMediaItem item)
        {
            InsertItem(index, item);
        }

        public new void Clear()
        {
            ClearItems();
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            AndroidQueue.Clear();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            AndroidQueue.RemoveAt(index);
        }

        protected override void InsertItem(int index, IMediaItem item)
        {
            base.InsertItem(index, item);

            AndroidQueue.Insert(index, GetQueueItem(item));
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            base.MoveItem(oldIndex, newIndex);

            var item = AndroidQueue[oldIndex];

            AndroidQueue.Remove(item);
            AndroidQueue.Insert(newIndex, item);
        }

        private static MediaSessionCompat.QueueItem GetQueueItem(IMediaItem mediaItem)
        {
            var description = new MediaDescriptionCompat.Builder()
                .SetMediaId("test")
                .SetMediaUri(null)
                .SetTitle("Title")
                .SetSubtitle("Subtitle (artist?)")
                .SetDescription("Description")
                .SetExtras(null)
                .SetIconBitmap(null)
                .SetIconUri(null)
                .Build();

            return new MediaSessionCompat.QueueItem(description, BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0));
        }
    }
}
