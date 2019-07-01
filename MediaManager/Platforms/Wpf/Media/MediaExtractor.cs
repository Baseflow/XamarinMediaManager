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
        protected Dictionary<string, string> RequestHeaders => CrossMediaManager.Current.RequestHeaders;

        public MediaExtractor()
        {
        }

        public Task<IMediaItem> CreateMediaItem(string url)
        {
            IMediaItem mediaItem = new MediaItem(url);
            return Task.FromResult(mediaItem);
        }

        public Task<IMediaItem> CreateMediaItem(FileInfo file)
        {
            return null;
        }

        public Task<IMediaItem> CreateMediaItem(IMediaItem mediaItem)
        {
            return Task.FromResult(mediaItem);
        }

        public Task<object> RetrieveMediaItemArt(IMediaItem mediaItem)
        {
            return null;
        }
    }
}
