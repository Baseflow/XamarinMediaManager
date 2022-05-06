using MediaManager.Library;

namespace MediaManager.Media
{
    public class VorbisCommentProvider : MediaExtractorProviderBase, IMediaItemMetadataProvider
    {
        public Task<IMediaItem> ProvideMetadata(IMediaItem mediaItem)
        {
            return null;
        }
    }
}
