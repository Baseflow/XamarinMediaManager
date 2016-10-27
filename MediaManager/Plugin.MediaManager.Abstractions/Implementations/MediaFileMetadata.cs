using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.MediaManager.Abstractions.Implementations
{
    public class MediaFileMetadata : IMediaFileMetadata
    {
        public string Titel { get; set; }
        public string Artist { get; set; }
        public object Cover { get; set; }
    }
}
