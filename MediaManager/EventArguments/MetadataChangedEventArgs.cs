using System;
using MediaManager.Media;

namespace MediaManager.Abstractions.EventArguments
{
    public class MetadataChangedEventArgs : EventArgs
    {
        public MetadataChangedEventArgs(IMediaItem metaData)
        {
            MetaData = metaData;
        }

        public IMediaItem MetaData { get; set; }
    }
}
