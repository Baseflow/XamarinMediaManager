using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MediaManager.Media
{
    public class MediaQueue : ObservableCollection<IMediaItem>, IMediaQueue
    {
        public event QueueEndedEventHandler QueueEnded;
        public event QueueMediaChangedEventHandler QueueMediaChanged;

        public bool HasNext()
        {
            throw new NotImplementedException();
        }

        public bool HasPrevious()
        {
            throw new NotImplementedException();
        }

        public IMediaItem Current => this[0];

        public int Index => throw new NotImplementedException();
    }
}
