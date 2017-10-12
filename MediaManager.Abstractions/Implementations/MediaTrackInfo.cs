

using System.ComponentModel;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager.Abstractions.Implementations
{
    public class MediaTrackInfo : IMediaTrackInfo
    {
        public string DisplayName { get; set; }

        public string TrackId { get; set; }
        public int? TrackIndex { get; set; }
        //public MediaFormat Format { get; }
        public string LanguageCode { get; set; }
        public string LanguageTag { get; set; }
        public MediaTrackType TrackType { get; set; }

        public object Tag { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            string st = $"{TrackIndex}, {TrackId}, {LanguageCode}, {LanguageTag}, {TrackType}";
            return st;
        }
    }
}
