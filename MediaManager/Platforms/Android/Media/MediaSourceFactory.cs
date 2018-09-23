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
    public class MediaSourceFactory : Java.Lang.Object, TimelineQueueEditor.IMediaSourceFactory
    {
        private DefaultDataSourceFactory _defaultDataSourceFactory;

        public MediaSourceFactory(DefaultDataSourceFactory factory)
        {
            _defaultDataSourceFactory = factory;
        }

        public MediaSourceFactory(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public IMediaSource CreateMediaSource(MediaDescriptionCompat description)
        {
            IMediaSource extractorMediaSource = null;

            if (description.MediaId != null)
                extractorMediaSource = null; //TODO: Implement preparefrommediasource

            else if (description.MediaUri != null)
            {
                extractorMediaSource = new ExtractorMediaSource.Factory(_defaultDataSourceFactory)
                    .SetTag(description)
                    .CreateMediaSource(description.MediaUri);
            }

            return extractorMediaSource;
        }
    }
}
