using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.MediaManager.Abstractions.Implementations
{
    public class MediaFileMetadata : IMediaFileMetadata
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public object Cover { get; set; }
    }
}
