using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MediaManager.Media
{
    public class MediaItem : NotifyPropertyChangedBase, IMediaItem
    {
        public MediaItem(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException(uri);
            MediaUri = uri;
        }

        public event MetadataUpdatedEventHandler MetadataUpdated;

        public string Advertisement
        {
            get => _advertisement;
            set => SetProperty(ref _advertisement, value);
        }
        public string Album
        {
            get => _album;
            set => SetProperty(ref _album, value);
        }
        public object AlbumArt
        {
            get => _albumArt;
            set => SetProperty(ref _albumArt, value);
        }
        public string AlbumArtist
        {
            get => _albumArtist;
            set => SetProperty(ref _albumArtist, value);
        }
        public string AlbumArtUri
        {
            get => _albumArtUri;
            set => SetProperty(ref _albumArtUri, value);
        }
        public object Art
        {
            get => _art;
            set => SetProperty(ref _art, value);
        }
        public string Artist
        {
            get => _artist;
            set => SetProperty(ref _artist, value);
        }
        public string ArtUri
        {
            get => _artUri;
            set => SetProperty(ref _artUri, value);
        }
        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }
        public BtFolderType BtFolderType
        {
            get => _btFolderType;
            set => SetProperty(ref _btFolderType, value);
        }
        public string Compilation
        {
            get => _compilation;
            set => SetProperty(ref _compilation, value);
        }
        public string Composer
        {
            get => _composer;
            set => SetProperty(ref _composer, value);
        }
        public string Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }
        public int DiscNumber
        {
            get => _discNumber;
            set => SetProperty(ref _discNumber, value);
        }
        public string DisplayDescription
        {
            get => _displayDescription;
            set => SetProperty(ref _displayDescription, value);
        }
        public object DisplayIcon
        {
            get => _displayIcon;
            set => SetProperty(ref _displayIcon, value);
        }
        public string DisplayIconUri
        {
            get => _displayIconUri;
            set => SetProperty(ref _displayIconUri, value);
        }
        public string DisplaySubtitle
        {
            get => _displaySubtitle;
            set => SetProperty(ref _displaySubtitle, value);
        }
        public string DisplayTitle
        {
            get => _displayTitle;
            set => SetProperty(ref _displayTitle, value);
        }
        public DownloadStatus DownloadStatus
        {
            get => _downloadStatus;
            set => SetProperty(ref _downloadStatus, value);
        }
        public TimeSpan Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }
        public object Extras
        {
            get => _extras;
            set => SetProperty(ref _extras, value);
        }
        public string Genre
        {
            get => _genre;
            set => SetProperty(ref _genre, value);
        }
        public string MediaId
        {
            get => _mediaId;
            set => SetProperty(ref _mediaId, value);
        }
        public string MediaUri
        {
            get => _mediaUri;
            set => SetProperty(ref _mediaUri, value);
        }
        public int NumTracks
        {
            get => _numTracks;
            set => SetProperty(ref _numTracks, value);
        }
        public object Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        public int TrackNumber
        {
            get => _trackNumber;
            set => SetProperty(ref _trackNumber, value);
        }
        public object UserRating
        {
            get => _userRating;
            set => SetProperty(ref _userRating, value);
        }
        public string Writer
        {
            get => _writer;
            set => SetProperty(ref _writer, value);
        }
        public int Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }

        public string FileExtension
        {
            get => _fileExtension;
            set => SetProperty(ref _fileExtension, value);
        }
        public MediaType MediaType
        {
            get => _mediaType;
            set => SetProperty(ref _mediaType, value);
        }
        public MediaLocation MediaLocation
        {
            get => _mediaLocation;
            set => SetProperty(ref _mediaLocation, value);
        }

        private bool _isMetadataExtracted = false;
        private MediaLocation _mediaLocation = MediaLocation.Unknown;
        private MediaType _mediaType = MediaType.Default;
        private string _fileExtension;
        private int _year;
        private string _writer;
        private object _userRating;
        private int _trackNumber;
        private string _title;
        private object _rating;
        private int _numTracks;
        private string _mediaUri;
        private string _mediaId = Guid.NewGuid().ToString();
        private string _genre;
        private object _extras;
        private TimeSpan _duration;
        private DownloadStatus _downloadStatus = DownloadStatus.NotDownloaded;
        private string _displayTitle;
        private string _displaySubtitle;
        private string _displayIconUri;
        private object _displayIcon;
        private string _displayDescription;
        private int _discNumber;
        private string _date;
        private string _composer;
        private string _compilation;
        private BtFolderType _btFolderType = BtFolderType.Mixed;
        private string _author;
        private string _artUri;
        private string _artist;
        private object _art;
        private string _albumArtUri;
        private string _albumArtist;
        private object _albumArt;
        private string _album;
        private string _advertisement;

        public bool IsMetadataExtracted
        {
            get
            {
                return _isMetadataExtracted;
            }
            set
            {
                if (SetProperty(ref _isMetadataExtracted, value))
                    MetadataUpdated?.Invoke(this, new MetadataChangedEventArgs(this));
            }
        }
    }
}
