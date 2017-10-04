using System;
using System.ComponentModel;

namespace Plugin.MediaManager.Abstractions.Implementations
{
    public class MediaItemMetadata : IMediaItemMetadata
    {
        public string Album { get; set; }

        public object AlbumArt { get; set; }

        public string AlbumArtist { get; set; }

        public string AlbumArtUri { get; set; }

        public object Art { get; set; }

        public string Artist { get; set; }

        public string ArtUri { get; set; }

        public string Author { get; set; }

        public string BluetoothFolderType { get; set; }

        public string Compilation { get; set; }

        public string Composer { get; set; }

        public DateTime Date { get; set; }

        public int DiscNumber { get; set; }

        public string DisplayDescription { get; set; }

        public object DisplayIcon { get; set; }

        public string DisplayIconUri { get; set; }

        public string DisplaySubtitle { get; set; }

        public string DisplayTitle { get; set; }

        public int Duration { get; set; }

        public string Genre { get; set; }

        public string MediaId { get; set; }

        public string MediaUri { get; set; }

        public int NumTracks { get; set; }

        public string Rating { get; set; }

        public string Title { get; set; }

        public int TrackNumber { get; set; }

        public string UserRating { get; set; }

        public string Writer { get; set; }

        public long Year { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
