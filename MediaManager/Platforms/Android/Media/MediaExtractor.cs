using System;
using System.Threading.Tasks;
using Android.Content.Res;
using Android.Graphics;
using Android.Media;
using MediaManager.Media;

namespace MediaManager.Platforms.Android.Media
{
    public class MediaExtractor : MediaExtractorBase, IMediaExtractor
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;
        protected Resources Resources => Resources.System;

        public MediaExtractor()
        {
        }

        public override async Task<IMediaItem> ExtractMetadata(IMediaItem mediaItem)
        {
            try
            {
                var metaRetriever = new MediaMetadataRetriever();

                switch (mediaItem.MediaLocation)
                {
                    case MediaLocation.Embedded:
                    case MediaLocation.FileSystem:
                        await metaRetriever.SetDataSourceAsync(mediaItem.MediaUri);
                        break;
                    default:
                        await metaRetriever.SetDataSourceAsync(mediaItem.MediaUri, RequestHeaders);
                        break;
                }

                return await ExtractMediaInfo(metaRetriever, mediaItem).ConfigureAwait(false);
            }
            catch
            {

            }
            return mediaItem;
        }

        protected virtual async Task<IMediaItem> ExtractMediaInfo(MediaMetadataRetriever mediaMetadataRetriever, IMediaItem mediaItem)
        {
            if (string.IsNullOrEmpty(mediaItem.Album))
                mediaItem.Album = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Album);

            if (string.IsNullOrEmpty(mediaItem.AlbumArtist))
                mediaItem.AlbumArtist = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Albumartist);

            if (string.IsNullOrEmpty(mediaItem.Artist))
                mediaItem.Artist = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Artist);

            if (string.IsNullOrEmpty(mediaItem.Author))
                mediaItem.Author = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Author);

            var trackNumber = mediaMetadataRetriever.ExtractMetadata(MetadataKey.CdTrackNumber);
            if (!string.IsNullOrEmpty(trackNumber) && int.TryParse(trackNumber, out var trackNumberResult))
                mediaItem.TrackNumber = trackNumberResult;

            if (string.IsNullOrEmpty(mediaItem.Compilation))
                mediaItem.Compilation = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Compilation);

            if (string.IsNullOrEmpty(mediaItem.Composer))
                mediaItem.Composer = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Composer);

            if (string.IsNullOrEmpty(mediaItem.Date))
                mediaItem.Date = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Date);

            var discNumber = mediaMetadataRetriever.ExtractMetadata(MetadataKey.DiscNumber);
            if (!string.IsNullOrEmpty(discNumber) && int.TryParse(discNumber, out var discNumberResult))
                mediaItem.DiscNumber = discNumberResult;

            var duration = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Duration);
            if (!string.IsNullOrEmpty(duration) && int.TryParse(duration, out var durationResult))
                mediaItem.Duration = TimeSpan.FromMilliseconds(durationResult);

            if (string.IsNullOrEmpty(mediaItem.Genre))
                mediaItem.Genre = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Genre);

            var numTracks = mediaMetadataRetriever.ExtractMetadata(MetadataKey.NumTracks);
            if (!string.IsNullOrEmpty(numTracks) && int.TryParse(numTracks, out var numTracksResult))
                mediaItem.NumTracks = numTracksResult;

            if (string.IsNullOrEmpty(mediaItem.Title))
                mediaItem.Title = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Title);

            if (string.IsNullOrEmpty(mediaItem.Writer))
                mediaItem.Writer = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Writer);

            var year = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Year);
            if (!string.IsNullOrEmpty(year) && int.TryParse(year, out var yearResult))
                mediaItem.Year = yearResult;

            byte[] imageByteArray = null;
            try
            {
                imageByteArray = mediaMetadataRetriever.GetEmbeddedPicture();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (imageByteArray == null)
            {
                mediaItem.AlbumArt = GetTrackCover(mediaItem);
            }
            else
            {
                try
                {
                    mediaItem.AlbumArt = await BitmapFactory.DecodeByteArrayAsync(imageByteArray, 0, imageByteArray.Length);
                }
                catch (Java.Lang.OutOfMemoryError)
                {
                    mediaItem.AlbumArt = null;
                }
            }

            mediaItem.IsMetadataExtracted = true;
            return mediaItem;
        }

        protected virtual Bitmap GetTrackCover(IMediaItem currentTrack)
        {
            var albumFolder = GetCurrentSongFolder(currentTrack);
            if (albumFolder == null)
                return null;

            if (!albumFolder.EndsWith("/"))
            {
                albumFolder += "/";
            }

            var baseUri = new System.Uri(albumFolder);
            var albumArtPath = TryGetAlbumArtPathByFilename(baseUri, "Folder.jpg");
            if (albumArtPath == null)
            {
                albumArtPath = TryGetAlbumArtPathByFilename(baseUri, "Cover.jpg");
                if (albumArtPath == null)
                {
                    albumArtPath = TryGetAlbumArtPathByFilename(baseUri, "AlbumArtSmall.jpg");
                    if (albumArtPath == null)
                        return null;
                }
            }

            var bitmap = BitmapFactory.DecodeFile(albumArtPath);
            return bitmap ?? BitmapFactory.DecodeResource(Resources, MediaManager.NotificationIconResource);
        }

        protected virtual string TryGetAlbumArtPathByFilename(System.Uri baseUri, string filename)
        {
            var testUri = new System.Uri(baseUri, filename);
            var testPath = testUri.LocalPath;
            if (System.IO.File.Exists(testPath))
                return testPath;
            else
                return null;
        }

        protected virtual string GetCurrentSongFolder(IMediaItem currentFile)
        {
            if (!new Uri(currentFile.MediaUri).IsFile)
                return null;

            return System.IO.Path.GetDirectoryName(currentFile.MediaUri);
        }

        public override Task<object> RetrieveMediaItemArt(IMediaItem mediaItem)
        {
            return null;
        }
    }
}
