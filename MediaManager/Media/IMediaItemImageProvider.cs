using MediaManager.Library;

namespace MediaManager.Media
{
    public interface IMediaItemImageProvider : IMediaExtractorProvider
    {
        Task<object> ProvideImage(IMediaItem mediaItem);
    }
}
