﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager.Platforms.Uap.Media
{
    public class MediaExtractor : MediaExtractorBase, IMediaExtractor
    {
        public MediaExtractor()
        {
        }

        public override IList<IProvider> CreateProviders()
        {
            var providers = base.CreateProviders();
            providers.Add(new StorageFileProvider());
            return providers;
        }

        protected override Task<string> GetResourcePath(string resourceName)
        {
            return null;
        }
    }
}
