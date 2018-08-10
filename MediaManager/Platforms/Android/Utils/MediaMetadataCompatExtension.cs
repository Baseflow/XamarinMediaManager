using android = Android;
using Android.Graphics;
using Android.Support.V4.Media;

namespace MediaManager.Platforms.Android.Utils
{
    public class MediaMetadataCompatExtension
    {
        MediaMetadataCompat metadata;

        public MediaMetadataCompatExtension(MediaMetadataCompat metadata)
        {
            this.metadata = metadata;
        }

        public string Id => metadata.GetString("android.media.metadata.MEDIA_ID");


        public string Title => metadata.GetString("android.media.metadata.TITLE");


        public string Artist => metadata.GetString("android.media.metadata.ARTIST");


        public long Duration => metadata.GetLong("android.media.metadata.DURATION");


        public string Album => metadata.GetString("android.media.metadata.ALBUM");


        public string Author => metadata.GetString("android.media.metadata.AUTHOR");


        public string Writer => metadata.GetString("android.media.metadata.WRITER");


        public string Composer => metadata.GetString("android.media.metadata.COMPOSER");


        public string Compilation => metadata.GetString("android.media.metadata.COMPILATION");


        public string Date => metadata.GetString("android.media.metadata.DATE");


        public string Year => metadata.GetString("android.media.metadata.YEAR");


        public string Genre => metadata.GetString("android.media.metadata.GENRE");


        public long TrackNumber => metadata.GetLong("android.media.metadata.TRACK_NUMBER");


        public long TrackCount => metadata.GetLong("android.media.metadata.NUM_TRACKS");


        public long DiscNumber => metadata.GetLong("android.media.metadata.DISC_NUMBER");


        public string AlbumArtist => metadata.GetString("android.media.metadata.ALBUM_ARTIST");


        public Bitmap Art => metadata.GetBitmap("android.media.metadata.ART");


        public android.Net.Uri ArtUri => android.Net.Uri.Parse(metadata.GetString("android.media.metadata.ART_URI"));


        public Bitmap AlbumArt => metadata.GetBitmap("android.media.metadata.ALBUM_ART");


        public android.Net.Uri AlbumArtUri => android.Net.Uri.Parse(metadata.GetString("android.media.metadata.ALBUM_ART_URI"));


        public long UserRating => metadata.GetLong("android.media.metadata.USER_RATING");


        public long Rating => metadata.GetLong("android.media.metadata.RATING");


        public string DisplayTitle => metadata.GetString("android.media.metadata.DISPLAY_TITLE");


        public string DisplaySubtitle => metadata.GetString("android.media.metadata.DISPLAY_SUBTITLE");


        public string DisplayDescription => metadata.GetString("android.media.metadata.DISPLAY_DESCRIPTION");


        public Bitmap DisplayIcon => metadata.GetBitmap("android.media.metadata.DISPLAY_ICON");


        public android.Net.Uri DisplayIconUri => android.Net.Uri.Parse(metadata.GetString("android.media.metadata.DISPLAY_ICON_URI"));


        public android.Net.Uri MediaUri => android.Net.Uri.Parse(metadata.GetString("android.media.metadata.MEDIA_URI"));


        public long DownloadStatus => metadata.GetLong("android.media.metadata.DOWNLOAD_STATUS");
    }
}
