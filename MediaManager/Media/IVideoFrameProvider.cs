using System;
using System.Threading.Tasks;
using MediaManager.Library;

namespace MediaManager.Media
{
    public interface IVideoFrameProvider : IProvider
    {
        Task<object> ProvideVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart);
    }
}
