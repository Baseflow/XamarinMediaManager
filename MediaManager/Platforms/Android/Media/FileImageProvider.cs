using System;
using System.Threading.Tasks;
using Android.Graphics;
using MediaManager.Media;

namespace MediaManager.Platforms.Android.Media
{
    public class FileImageProvider : IImageProvider
    {
        public Task<object> ProvideImage(IMediaItem mediaItem)
        {
            var albumFolder = GetCurrentSongFolder(mediaItem);
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
            return Task.FromResult(bitmap as object);
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
    }
}
