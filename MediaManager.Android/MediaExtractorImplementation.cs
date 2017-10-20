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
        private MediaManagerImplementation _mediaManagerImplementation;

        public MediaExtractorImplementation(MediaManagerImplementation mediaManagerImplementation)
        {
            this._mediaManagerImplementation = mediaManagerImplementation;
        }

        public Task<IMediaItem> ExtractMediaInfo(IMediaItem item)
        {
           return _mediaManagerImplementation?.MediaExtractor.ExtractMediaInfo(item);
        }
    }
}
