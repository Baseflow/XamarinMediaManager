using System;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager.Tests
{
    class MediaFile : IMediaFile
    {
        public int Id { get; set; }
        public MediaFileType Type { get; set; }
        public string Url { get; set; }

        public override string ToString()
        {
            return string.Format("[MediaFile: Id={0}, Type={1}, Url={2}]", Id, Type, Url);
        }
    }
}
