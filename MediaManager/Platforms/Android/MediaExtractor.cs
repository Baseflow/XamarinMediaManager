using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Android.Content.Res;
using Android.Graphics;
using Android.Media;
using Android.Support.V4.Media;
using MediaManager.Media;

namespace MediaManager.Platforms.Android
{
    public class MediaExtractor : IMediaExtractor
    {
        protected Resources Resources => Resources.System;
        protected Dictionary<string, string> RequestHeaders => CrossMediaManager.Current.RequestHeaders;

        public MediaExtractor()
        {
        }

        public virtual async Task<IMediaItem> CreateMediaItem(string url)
        {
            var metaRetriever = new MediaMetadataRetriever();
            await metaRetriever.SetDataSourceAsync(url, RequestHeaders);

            var mediaItem = new MediaItem(url);
            return await ExtractMediaInfo(metaRetriever, mediaItem);
        }

        public virtual async Task<IMediaItem> CreateMediaItem(FileInfo file)
        {
            var metaRetriever = new MediaMetadataRetriever();

            var javaFile = new Java.IO.File(file.FullName);
            var inputStream = new Java.IO.FileInputStream(javaFile);
            await metaRetriever.SetDataSourceAsync(inputStream.FD);

            var mediaItem = new MediaItem(file.FullName);
            return await ExtractMediaInfo(metaRetriever, mediaItem);
        }

        public virtual async Task<IMediaItem> CreateMediaItem(IMediaItem mediaItem)
        {
            var metaRetriever = new MediaMetadataRetriever();
            await metaRetriever.SetDataSourceAsync(mediaItem.MediaUri, RequestHeaders);

            return await ExtractMediaInfo(metaRetriever, mediaItem);
        }

        protected async Task<IMediaItem> ExtractMediaInfo(MediaMetadataRetriever mediaMetadataRetriever, IMediaItem mediaItem)
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
            if (!string.IsNullOrEmpty(trackNumber) && int.TryParse(trackNumber, out int trackNumberResult))
                mediaItem.TrackNumber = trackNumberResult;

            if (string.IsNullOrEmpty(mediaItem.Compilation))
                mediaItem.Compilation = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Compilation);

            if (string.IsNullOrEmpty(mediaItem.Composer))
                mediaItem.Composer = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Composer);

            if (string.IsNullOrEmpty(mediaItem.Date))
                mediaItem.Date = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Date);

            var discNumber = mediaMetadataRetriever.ExtractMetadata(MetadataKey.DiscNumber);
            if (!string.IsNullOrEmpty(discNumber) && int.TryParse(discNumber, out int discNumberResult))
                mediaItem.DiscNumber = discNumberResult;

            var duration = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Duration);
            if (!string.IsNullOrEmpty(duration) && int.TryParse(duration, out int durationResult))
                mediaItem.Duration = TimeSpan.FromMilliseconds(durationResult);

            if (string.IsNullOrEmpty(mediaItem.Genre))
                mediaItem.Genre = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Genre);

            var numTracks = mediaMetadataRetriever.ExtractMetadata(MetadataKey.NumTracks);
            if (!string.IsNullOrEmpty(numTracks) && int.TryParse(numTracks, out int numTracksResult))
                mediaItem.NumTracks = numTracksResult;

            if (string.IsNullOrEmpty(mediaItem.Title))
                mediaItem.Title = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Title);

            if (string.IsNullOrEmpty(mediaItem.Writer))
                mediaItem.Writer = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Writer);

            var year = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Year);
            if (!string.IsNullOrEmpty(year) && int.TryParse(year, out int yearResult))
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

        public object GetMediaCover(IMediaItem mediaItem)
        {
            //TODO: move cover in here
            return null;
        }

        protected Bitmap GetTrackCover(IMediaItem currentTrack)
        {
            string albumFolder = GetCurrentSongFolder(currentTrack);
            if (albumFolder == null)
                return null;

            if (!albumFolder.EndsWith("/"))
            {
                albumFolder += "/";
            }

            System.Uri baseUri = new System.Uri(albumFolder);
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

            Bitmap bitmap = BitmapFactory.DecodeFile(albumArtPath);
            return bitmap ?? BitmapFactory.DecodeResource(Resources, Resource.Drawable.exo_notification_play);
        }

        protected string TryGetAlbumArtPathByFilename(System.Uri baseUri, string filename)
        {
            System.Uri testUri = new System.Uri(baseUri, filename);
            string testPath = testUri.LocalPath;
            if (System.IO.File.Exists(testPath))
                return testPath;
            else
                return null;
        }

        protected string GetCurrentSongFolder(IMediaItem currentFile)
        {
            if (!new Uri(currentFile.MediaUri).IsFile)
                return null;

            return System.IO.Path.GetDirectoryName(currentFile.MediaUri);
        }
    }
}
