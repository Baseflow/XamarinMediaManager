using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager.Platforms.Wpf.Media
{
    public class MediaExtractor : MediaExtractorBase, IMediaExtractor
    {
        public override Task<IMediaItem> ExtractMetadata(IMediaItem mediaItem)
        {
            return Task.FromResult(mediaItem);
        }

        public override Task<object> RetrieveMediaItemArt(IMediaItem mediaItem)
        {
            return null;
        }
    }
}
