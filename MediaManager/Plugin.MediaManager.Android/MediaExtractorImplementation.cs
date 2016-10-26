using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.Content.Res;
using Android.Graphics;
using Android.Media;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class MediaExtractorImplementation : IMediaExtractor
    {
        IMediaFile File;

        public async Task<IMediaFile> ExtractMediaInfo(IMediaFile mediaFile)
        {
            File = mediaFile;
            var metaDataRetriever = await GetMetadataRetriever();

            //TODO: Add metaDataRetriever properties to File

            return File;
        }

        private async Task<MediaMetadataRetriever> GetMetadataRetriever()
        {
            MediaMetadataRetriever metaRetriever = new MediaMetadataRetriever();

            switch (File.Type)
            {
                case MediaFileType.AudioUrl:
                    await metaRetriever.SetDataSourceAsync(File.Url, new Dictionary<string, string>());
                    break;
                case MediaFileType.VideoUrl:
                    break;
                case MediaFileType.AudioFile:
                    Java.IO.File file = new Java.IO.File(File.Url);
                    Java.IO.FileInputStream inputStream = new Java.IO.FileInputStream(file);
                    await metaRetriever.SetDataSourceAsync(inputStream.FD);
                    break;
                case MediaFileType.VideoFile:
                    break;
                case MediaFileType.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            return metaRetriever;
        }

        private string TryGetAlbumArtPathByFilename(System.Uri baseUri, string filename)
        {
            System.Uri testUri = new System.Uri(baseUri, filename);
            string testPath = testUri.LocalPath;
            if (System.IO.File.Exists(testPath))
                return testPath;
            else
                return null;
        }

        private async Task GetCoverFromMetaData(MediaMetadataRetriever metaRetriever)
        {
            byte[] imageByteArray = metaRetriever.GetEmbeddedPicture();
            if (imageByteArray == null)
            {
                Bitmap coverBitmap = GetCurrentTrackCover();
                if (coverBitmap != null)
                    File.Cover = coverBitmap;
                else
                    File.Cover = await BitmapFactory.DecodeResourceAsync(Resources, Resource.Drawable.ButtonStar);
            }
            else
            {
                try
                {
                    File.Cover = await BitmapFactory.DecodeByteArrayAsync(imageByteArray, 0, imageByteArray.Length);
                }
                catch (Java.Lang.OutOfMemoryError ex)
                {
                    File.Cover = null;
                    //MediaFileFailed?.Invoke(this, new MediaFileFailedEventArgs(ex, mediaFile));
                }
            }
        }

        private Bitmap GetCurrentTrackCover()
        {
            string albumFolder = GetCurrentSongFolder();
            if (albumFolder == null)
                return null;

            if (!albumFolder.EndsWith("/"))
            {
                albumFolder += "/";
            }

            System.Uri baseUri = new System.Uri(albumFolder);
            string albumArtPath;
            albumArtPath = TryGetAlbumArtPathByFilename(baseUri, "Folder.jpg");
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

            return bitmap;
        }

        private string GetCurrentSongFolder()
        {
            if (!new System.Uri(File.Url).IsFile)
                return null;

            return System.IO.Path.GetDirectoryName(File.Url);
        }
    }
}
