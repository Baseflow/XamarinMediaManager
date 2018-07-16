using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager.Platforms.Android
{
    public class MediaExtractor : IMediaExtractor
    {
        public virtual Task<IMediaItem> CreateMediaItem(string url)
        {
            var item = new MediaItem() { MetadataMediaUri = url } as IMediaItem;
            return Task.FromResult(item);
        }

        public virtual Task<IMediaItem> CreateMediaItem(FileInfo file)
        {
            throw new NotImplementedException();
        }
    }
}
