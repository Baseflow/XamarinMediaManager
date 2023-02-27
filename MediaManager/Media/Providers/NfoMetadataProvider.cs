using MediaManager.Library;

namespace MediaManager.Media
{
    public class NfoMetadataProvider : MediaExtractorProviderBase, IMediaItemMetadataProvider
    {
        public Task<IMediaItem> ProvideMetadata(IMediaItem mediaItem)
        {
            return Task.FromResult<IMediaItem>(null);
        }
    }
}
