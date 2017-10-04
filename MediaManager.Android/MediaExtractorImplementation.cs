using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.Content.Res;
using Android.Graphics;
using Android.Media;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class MediaExtractorImplementation : IMediaExtractor
    {
        private MediaManagerImplementation mediaManagerImplementation;

        public MediaExtractorImplementation(MediaManagerImplementation mediaManagerImplementation)
        {
            this.mediaManagerImplementation = mediaManagerImplementation;
        }

        public Task<IMediaItem> ExtractMediaInfo(IMediaItem item)
        {
            throw new NotImplementedException();
        }
    }
}
