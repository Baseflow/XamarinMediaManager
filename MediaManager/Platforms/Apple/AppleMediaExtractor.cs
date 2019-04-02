using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager.Platforms.Apple
{
    public class AppleMediaExtractor : IMediaExtractor
    {
        protected Dictionary<string, string> RequestHeaders => CrossMediaManager.Current.RequestHeaders;

        public AppleMediaExtractor()
        {
        }

        public virtual async Task<IMediaItem> CreateMediaItem(string url)
        {
            IMediaItem mediaItem = new MediaItem(url);
            // TODO: Get metadata!
            return mediaItem;
        }

        public virtual async Task<IMediaItem> CreateMediaItem(FileInfo file)
        {
            IMediaItem mediaItem = new MediaItem(file.FullName);
            // TODO: Get metadata!
            return mediaItem;
        }

        public virtual async Task<IMediaItem> CreateMediaItem(IMediaItem mediaItem)
        {
            // TODO: Get metadata!
            return mediaItem;
        }
    }
}
