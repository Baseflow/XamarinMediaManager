using System;
using System.IO;
using MediaManager.Media;

namespace MediaManager.Library
{
    public class MediaItem : ContentItem, IMediaItem
    {
        public MediaItem()
        {
        }

        public MediaItem(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException(uri);
            MediaUri = uri;
        }

        public event MetadataUpdatedEventHandler MetadataUpdated;

        private string _advertisement;
        public string Advertisement
        {
            get => _advertisement;
            set => SetProperty(ref _advertisement, value);
        }

        private string _album;
        public string Album
        {
            get => _album;
            set => SetProperty(ref _album, value);
        }

        private object _albumArt;
        public object AlbumImage
        {
            get => _albumArt;
            set => SetProperty(ref _albumArt, value);
        }

        private string _albumArtist;
        public string AlbumArtist
        {
            get => _albumArtist;
            set => SetProperty(ref _albumArtist, value);
        }

        private string _albumArtUri;
        public string AlbumImageUri
        {
            get => _albumArtUri;
            set => SetProperty(ref _albumArtUri, value);
        }

        private object _art;
        public object Image
        {
            get => _art;
            set => SetProperty(ref _art, value);
        }

        private string _artist;
        public string Artist
        {
            get => _artist;
            set => SetProperty(ref _artist, value);
        }

        private string _artUri;
        public string ImageUri
        {
            get => _artUri;
            set => SetProperty(ref _artUri, value);
        }

        private string _author;
        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        private string _compilation;
        public string Compilation
        {
            get => _compilation;
            set => SetProperty(ref _compilation, value);
        }

        private string _composer;
        public string Composer
        {
            get => _composer;
            set => SetProperty(ref _composer, value);
        }

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        private int _discNumber;
        public int DiscNumber
        {
            get => _discNumber;
            set => SetProperty(ref _discNumber, value);
        }

        private object _displayImage;
        public object DisplayImage
        {
            get
            {
                if (_displayImage != null)
                    return _displayImage;
                if (Image != null)
                    return Image;
                else if (AlbumImage != null)
                    return AlbumImage;
                else
                    return null;
            }

            set => SetProperty(ref _displayImage, value);
        }

        private string _displayImageUri;
        public string DisplayImageUri
        {
            get
            {
                if (!string.IsNullOrEmpty(_displayImageUri))
                    return _displayImageUri;
                if (!string.IsNullOrEmpty(ImageUri))
                    return ImageUri;
                else if (!string.IsNullOrEmpty(AlbumImageUri))
                    return AlbumImageUri;
                else
                    return string.Empty;
            }

            set => SetProperty(ref _displayImageUri, value);
        }

        private string _displayTitle;
        public string DisplayTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(_displayTitle))
                    return _displayTitle;
                else if (!string.IsNullOrEmpty(Title))
                    return Title;
                else if (!string.IsNullOrEmpty(FileName))
                    return FileName;
                else
                    return string.Empty;
            }

            set => SetProperty(ref _displayTitle, value);
        }

        private string _displaySubtitle;
        public string DisplaySubtitle
        {
            get
            {
                if (!string.IsNullOrEmpty(_displaySubtitle))
                    return _displaySubtitle;
                else if (!string.IsNullOrEmpty(Artist))
                    return Artist;
                else if (!string.IsNullOrEmpty(AlbumArtist))
                    return AlbumArtist;
                else if (!string.IsNullOrEmpty(Album))
                    return Album;
                else
                    return string.Empty;
            }

            set => SetProperty(ref _displaySubtitle, value);
        }

        private string _displayDescription;
        public string DisplayDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(_displayDescription))
                    return _displayDescription;
                else if (!string.IsNullOrEmpty(Album))
                    return Album;
                else if (!string.IsNullOrEmpty(Artist))
                    return Artist;
                else if (!string.IsNullOrEmpty(AlbumArtist))
                    return AlbumArtist;
                else
                    return string.Empty;
            }

            set => SetProperty(ref _displayDescription, value);
        }

        private DownloadStatus _downloadStatus = DownloadStatus.Unknown;
        public DownloadStatus DownloadStatus
        {
            get => _downloadStatus;
            set => SetProperty(ref _downloadStatus, value);
        }

        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        private object _extras;
        public object Extras
        {
            get => _extras;
            set => SetProperty(ref _extras, value);
        }

        private string _genre;
        public string Genre
        {
            get => _genre;
            set => SetProperty(ref _genre, value);
        }

        private string _mediaUri;
        public string MediaUri
        {
            get => _mediaUri;
            set => SetProperty(ref _mediaUri, value);
        }

        private int _numTracks;
        public int NumTracks
        {
            get => _numTracks;
            set => SetProperty(ref _numTracks, value);
        }

        private object _rating;
        public object Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private int _trackNumber;
        public int TrackNumber
        {
            get => _trackNumber;
            set => SetProperty(ref _trackNumber, value);
        }

        private object _userRating;
        public object UserRating
        {
            get => _userRating;
            set => SetProperty(ref _userRating, value);
        }

        private string _writer;
        public string Writer
        {
            get => _writer;
            set => SetProperty(ref _writer, value);
        }

        private int _year;
        public int Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }

        private string _fileExtension;
        public string FileExtension
        {
            get => _fileExtension;
            set => SetProperty(ref _fileExtension, value);
        }

        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        private MediaType _mediaType = MediaType.Default;
        public MediaType MediaType
        {
            get => _mediaType;
            set => SetProperty(ref _mediaType, value);
        }

        private MediaLocation _mediaLocation = MediaLocation.Unknown;
        public MediaLocation MediaLocation
        {
            get => _mediaLocation;
            set => SetProperty(ref _mediaLocation, value);
        }

        private bool _isMetadataExtracted = false;
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

        private bool _isLive = false;
        public bool IsLive
        {
            get => _isLive;
            set => SetProperty(ref _isLive, value);
        }

        private Stream _data;

        public Stream Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }
    }
}
