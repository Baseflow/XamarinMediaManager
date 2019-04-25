using MediaManager.Media;

namespace MediaManager
{
    public interface IMediaLibrary
    {
        IMediaList Items { get; set; }
    }
}
