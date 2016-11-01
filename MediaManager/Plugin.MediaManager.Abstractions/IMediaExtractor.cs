using System;
using System.Threading.Tasks;

namespace Plugin.MediaManager.Abstractions
{
    public interface IMediaExtractor
    {
        Task<IMediaFile> ExtractMediaInfo(IMediaFile mediaFile);
    }
}
