using System;
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

        public override object GetFrame(IMediaItem mediaItem, TimeSpan time)
        {
            throw new NotImplementedException();
        }

        public override Task<object> RetrieveMediaItemArt(IMediaItem mediaItem)
        {
            return null;
        }
    }
}
