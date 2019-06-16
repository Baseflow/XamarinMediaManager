using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager.Library
{
    public class MediaLibrary : IMediaLibrary
    {
        public Task<MediaItem> GetItem(string mediaId)
        {
            throw new NotImplementedException();
        }

        public Task<List<MediaItem>> GetItems()
        {
            throw new NotImplementedException();
        }

        public Task<MediaItem> SaveItem(MediaItem mediaItem)
        {
            throw new NotImplementedException();
        }
    }
}
