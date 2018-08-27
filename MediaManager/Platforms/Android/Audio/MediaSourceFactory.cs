using System;
using System.Collections.Generic;
using System.Text;
using Android.Support.V4.Media;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Upstream;

namespace MediaManager.Platforms.Android.Audio
{
    public class MediaSourceFactory : Java.Lang.Object, TimelineQueueEditor.IMediaSourceFactory
    {
        private DefaultDataSourceFactory factory;

        public MediaSourceFactory(DefaultDataSourceFactory factory)
        {
            this.factory = factory;
        }

        public IMediaSource CreateMediaSource(MediaDescriptionCompat description)
        {
            IMediaSource src = null;

            if (description.MediaId != null)
                src = null; //TODO: Implement preparefrommediasource

            else if (description.MediaUri != null)
                src = new ExtractorMediaSource(description.MediaUri, factory, new DefaultExtractorsFactory(), null, null);

            return src;
        }
    }
}
