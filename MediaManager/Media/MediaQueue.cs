using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MediaManager.Media
{
    public class MediaQueue : ObservableCollection<IMediaItem>, IMediaQueue
    {
    }
}
