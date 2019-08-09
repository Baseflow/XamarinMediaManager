using System;
using System.Threading.Tasks;
using MediaManager.Library;

namespace MediaManager.Media
{
    public interface IMediaItemVideoFrameProvider : IMediaExtractorProvider
    {
        Task<object> ProvideVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart);
    }
}
