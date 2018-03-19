using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class MetadataChangedEventArgs : EventArgs
    {
        public MetadataChangedEventArgs(IMediaFileMetadata metaData)
        {
            MetaData = metaData;
        }

        public IMediaFileMetadata MetaData { get; set; }
    }
}