
using System.ComponentModel;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager.Abstractions
{
    public interface IMediaTrackInfo : INotifyPropertyChanged
    {
        string DisplayName { get; set; }

        string TrackId { get; }
        int? TrackIndex { get; }

        string LanguageCode { get; }
        string LanguageTag { get; }
        MediaTrackType TrackType { get; }

        object Tag { get; set; }
    }
}
