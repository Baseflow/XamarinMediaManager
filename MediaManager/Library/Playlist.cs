using System;
using System.Collections.ObjectModel;
using MediaManager.Media;

namespace MediaManager.Library
{
    public class Playlist : ObservableCollection<IMediaItem>, IPlaylist
    {
        private string _playlistId = Guid.NewGuid().ToString();
        private string _uri;
        private string _title;
        private string _description;
        private string _tags;
        private string _genre;
        private object _art;
        private string _artUri;
        private object _rating;
        private DateTime _createdAt;
        private DateTime _updatedAt;
        private TimeSpan _totalTime;
        private SharingType _sharingType = SharingType.Public;
        private DownloadStatus _downloadStatus = DownloadStatus.NotDownloaded;

        public string PlaylistId
        {
            get => _playlistId;
            set => _playlistId = value;
        }
        public string Uri
        {
            get => _uri;
            set => _uri = value;
        }
        public string Title
        {
            get => _title;
            set => _title = value;
        }
        public string Description
        {
            get => _description;
            set => _description = value;
        }
        public string Tags
        {
            get => _tags;
            set => _tags = value;
        }
        public string Genre
        {
            get => _genre;
            set => _genre = value;
        }
        public object Art
        {
            get => _art;
            set => _art = value;
        }
        public string ArtUri
        {
            get => _artUri;
            set => _artUri = value;
        }
        public object Rating
        {
            get => _rating;
            set => _rating = value;
        }
        public DateTime CreatedAt
        {
            get => _createdAt;
            set => _createdAt = value;
        }
        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set => _updatedAt = value;
        }
        public TimeSpan TotalTime
        {
            get => _totalTime;
            set => _totalTime = value;
        }
        public SharingType SharingType
        {
            get => _sharingType;
            set => _sharingType = value;
        }
        public DownloadStatus DownloadStatus
        {
            get => _downloadStatus;
            set => _downloadStatus = value;
        }
    }
}
