using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MediaManager.Media
{
    public interface IMediaQueue : IList<IMediaItem>, INotifyCollectionChanged, INotifyPropertyChanged
    {
    }
}
