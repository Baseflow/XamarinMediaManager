using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager.Media
{
    public interface IMediaList : IList<IMediaItem>
    {
        string Title { get; set; }
    }
}
