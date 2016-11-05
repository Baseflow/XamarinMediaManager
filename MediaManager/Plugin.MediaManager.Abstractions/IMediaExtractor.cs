using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.MediaManager.Abstractions
{
    public interface IMediaExtractor
    {
        Task<IMediaFile> ExtractMediaInfo(IMediaFile mediaFile);
    }
}
