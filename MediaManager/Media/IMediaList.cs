using System.Collections.Generic;

namespace MediaManager.Media
{
    public interface IMediaList : IList<IMediaItem>
    {
        string Title { get; set; }
    }
}
