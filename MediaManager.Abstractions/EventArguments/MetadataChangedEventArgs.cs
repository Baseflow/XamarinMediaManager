using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class MetadataChangedEventArgs : EventArgs
    {
        public MetadataChangedEventArgs(IMediaItemMetadata metaData)
        {
            MetaData = metaData;
        }

        public IMediaItemMetadata MetaData { get; set; }
    }
}