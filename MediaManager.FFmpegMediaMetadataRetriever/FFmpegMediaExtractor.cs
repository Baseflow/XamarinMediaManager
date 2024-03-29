﻿using MediaManager.Media;
using MediaManager.Platforms.Android.Media;

namespace MediaManager.FFmpegMediaMetadataRetriever
{
    public class FFmpegMediaExtractor : MediaExtractor
    {
        public override IList<IMediaExtractorProvider> CreateProviders()
        {
            var providers = base.CreateProviders();
            providers.Insert(0, new FFmpegMetadataProvider());
            return providers;
        }
    }
}
