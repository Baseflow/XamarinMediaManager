using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaManager.Library
{
    public interface IMediaLibrary
    {
        Task<IEnumerable<IMediaItem>> GetItems();
        Task<IMediaItem> GetItem(string mediaId);
        Task<IMediaItem> SaveItem(IMediaItem mediaItem);
    }
}
