using MediaManager.Library;

namespace MediaManager.Media
{
    public class XmlMetadataProvider : MediaExtractorProviderBase, IMediaItemMetadataProvider
    {
        public Task<IMediaItem> ProvideMetadata(IMediaItem mediaItem)
        {
            return null;
        }
    }
}
