using System;
using System.Collections.Generic;
using System.Text;
using Android.Runtime;
using Android.Support.V4.Media;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Upstream;

namespace MediaManager.Platforms.Android.Media
{
    public class QueueEditorMediaSourceFactory : Java.Lang.Object, TimelineQueueEditor.IMediaSourceFactory
    {
        public QueueEditorMediaSourceFactory()
        {
        }

        public QueueEditorMediaSourceFactory(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public IMediaSource CreateMediaSource(MediaDescriptionCompat description)
        {
            return description?.ToMediaItem()?.ToMediaSource();
        }
    }
}
