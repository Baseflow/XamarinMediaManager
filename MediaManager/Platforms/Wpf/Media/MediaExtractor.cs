using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager.Platforms.Wpf.Media
{
    public class MediaExtractor : IMediaExtractor
    {
        public Task<IMediaItem> CreateMediaItem(string url)
        {
            throw new NotImplementedException();
        }

        public Task<IMediaItem> CreateMediaItem(FileInfo file)
        {
            throw new NotImplementedException();
        }

        public Task<IMediaItem> CreateMediaItem(IMediaItem mediaItem)
        {
            throw new NotImplementedException();
        }

        public Task<object> RetrieveMediaItemArt(IMediaItem mediaItem)
        {
            throw new NotImplementedException();
        }
    }
}
