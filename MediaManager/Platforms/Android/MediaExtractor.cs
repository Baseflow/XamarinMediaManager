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
        public virtual async Task<IMediaItem> CreateMediaItem(string url)
        {
            var metaRetriever = new MediaMetadataRetriever();
            await metaRetriever.SetDataSourceAsync(url, _requestHeaders);

            return await ExtractMediaInfo(metaRetriever, url);
        }

        public virtual async Task<IMediaItem> CreateMediaItem(FileInfo file)
        {
            var metaRetriever = new MediaMetadataRetriever();

            var javaFile = new Java.IO.File(file.FullName);
            var inputStream = new Java.IO.FileInputStream(javaFile);
            await metaRetriever.SetDataSourceAsync(inputStream.FD);

            return await ExtractMediaInfo(metaRetriever, file.FullName);
        }

        private readonly Resources _resources;
        private readonly Dictionary<string, string> _requestHeaders;

        public MediaExtractor(Resources resources, Dictionary<string, string> requestHeaders)
        {
            _resources = resources;
            _requestHeaders = requestHeaders;
        }

        public async Task<IMediaItem> ExtractMediaInfo(MediaMetadataRetriever mediaMetadataRetriever, string url)
        {
            var mediaFile = new MediaItem();
            mediaFile.MetadataMediaUri = url;
            mediaFile.MetadataAlbum = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Album);
            mediaFile.MetadataAlbumArtist = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Albumartist);
            mediaFile.MetadataArtist = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Artist);
            mediaFile.MetadataAuthor = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Author);
            mediaFile.MetadataTrackNumber = mediaMetadataRetriever.ExtractMetadata(MetadataKey.CdTrackNumber);
            mediaFile.MetadataCompilation = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Compilation);
            mediaFile.MetadataComposer = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Composer);
            mediaFile.MetadataDate = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Date);
            mediaFile.MetadataDiscNumber = mediaMetadataRetriever.ExtractMetadata(MetadataKey.DiscNumber);
            mediaFile.MetadataDuration = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Duration);
            mediaFile.MetadataGenre = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Genre);
            mediaFile.MetadataNumTracks = mediaMetadataRetriever.ExtractMetadata(MetadataKey.NumTracks);
            mediaFile.MetadataTitle = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Title);
            mediaFile.MetadataWriter = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Writer);
            mediaFile.MetadataYear = mediaMetadataRetriever.ExtractMetadata(MetadataKey.Year);

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
                mediaFile.MetadataAlbumArt = GetTrackCover(mediaFile);
            }
            else
            {
                try
                {
                    mediaFile.MetadataAlbumArt = await BitmapFactory.DecodeByteArrayAsync(imageByteArray, 0, imageByteArray.Length);
                }
                catch (Java.Lang.OutOfMemoryError)
                {
                    mediaFile.MetadataAlbumArt = null;
                }
            }
            return mediaFile;
        }

        private Bitmap GetTrackCover(IMediaItem currentTrack)
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
            return bitmap ?? BitmapFactory.DecodeResource(_resources, Resource.Drawable.exo_notification_play);
        }

        private static string TryGetAlbumArtPathByFilename(System.Uri baseUri, string filename)
        {
            System.Uri testUri = new System.Uri(baseUri, filename);
            string testPath = testUri.LocalPath;
            if (System.IO.File.Exists(testPath))
                return testPath;
            else
                return null;
        }

        private string GetCurrentSongFolder(IMediaItem currentFile)
        {
            if (!new System.Uri(currentFile.MetadataMediaUri).IsFile)
                return null;

            return System.IO.Path.GetDirectoryName(currentFile.MetadataMediaUri);
        }
    }
}
