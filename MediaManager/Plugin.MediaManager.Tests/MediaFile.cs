using System;
using System.ComponentModel;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager.Tests
{
    class MediaFile : IMediaFile
    {
        public int Id { get; set; }
        public MediaFileType Type { get; set; }
        public string Url { get; set; }
        public IMediaFileMetadata Metadata { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return string.Format("[MediaFile: Id={0}, Type={1}, Url={2}]", Id, Type, Url);
        }
        public bool MetadataExtracted { get; set; }
    }
}
