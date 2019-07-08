using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager.Library
{
    public class MediaLibrary : IMediaLibrary
    {
        public Task<IMediaItem> GetItem(string mediaId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IMediaItem>> GetItems()
        {
            throw new NotImplementedException();
        }

        public Task<IMediaItem> SaveItem(IMediaItem mediaItem)
        {
            throw new NotImplementedException();
        }
    }
}
