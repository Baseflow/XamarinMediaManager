using MediaManager.Media;

namespace MediaManager.Library
{
    public interface IMediaLibrary
    {
        IMediaList Items { get; set; }
    }
}
