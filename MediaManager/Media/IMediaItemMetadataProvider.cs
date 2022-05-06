using MediaManager.Library;

namespace MediaManager.Media
{
    public interface IMediaItemMetadataProvider : IMediaExtractorProvider
    {
        Task<IMediaItem> ProvideMetadata(IMediaItem mediaItem);
    }
}
