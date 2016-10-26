using System;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    public class MediaExtractorImplementation : IMediaExtractor
    {
        public Task<IMediaFile> ExtractMediaInfo(IMediaFile mediaFile)
        {
            return Task.FromResult(mediaFile);
        }
    }
}
