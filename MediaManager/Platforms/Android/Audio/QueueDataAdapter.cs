using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Runtime;
using Android.Support.V4.Media;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;

namespace MediaManager.Platforms.Android.Audio
{
    public class QueueDataAdapter : Java.Lang.Object, TimelineQueueEditor.IQueueDataAdapter
    {
        private IMediaManager mediaManager = CrossMediaManager.Current;

        public QueueDataAdapter()
        {
        }

        public QueueDataAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public void Add(int index, MediaDescriptionCompat description)
        {
            //mediaManager.MediaQueue.Insert(index);
        }

        public MediaDescriptionCompat GetMediaDescription(int index)
        {
            return mediaManager.MediaQueue.ElementAt(index).GetMediaDescription();
        }

        public void Move(int oldIndex, int newIndex)
        {
            //_queue.Move(oldIndex, newIndex);
        }

        public void Remove(int index)
        {
            mediaManager.MediaQueue.RemoveAt(index);
        }
    }
}
