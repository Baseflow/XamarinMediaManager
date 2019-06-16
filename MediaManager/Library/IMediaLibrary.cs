using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager.Library
{
    public interface IMediaLibrary
    {
        Task<List<MediaItem>> GetItems();
        Task<MediaItem> GetItem(string mediaId);
        Task<MediaItem> SaveItem(MediaItem mediaItem);
    }
}
