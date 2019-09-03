using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Library;

namespace MediaManager.Media
{
    public interface IMediaItemProvider
    {
        Task<IEnumerable<IMediaItem>> GetMediaÍtems(string search);
        Task<IMediaItem> GetMediaItem(Guid id);
        Task AddOrUpdateMediaItem(IMediaItem mediaItem);
    }
}
