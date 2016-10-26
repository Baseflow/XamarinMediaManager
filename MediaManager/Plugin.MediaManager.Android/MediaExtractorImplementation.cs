using System;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    public class MediaExtractorImplementation : IMediaExtractor
    {
        public async Task<IMediaFile> ExtractMediaInfo(IMediaFile mediaFile)
        {
            throw new NotImplementedException();
        }
    }
}
