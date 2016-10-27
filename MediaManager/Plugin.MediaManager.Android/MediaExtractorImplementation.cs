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
        private Resources _resources;
        public MediaExtractorImplementation(Resources resources)
        {
            _resources = resources;
        }

        public async Task<IMediaFile> ExtractMediaInfo(IMediaFile mediaFile)
        {
            MediaMetadataRetriever metaRetriever = await GetMetadataRetriever(mediaFile);
            SetMetadata(mediaFile, metaRetriever);
            byte[] imageByteArray = metaRetriever.GetEmbeddedPicture();
            if (imageByteArray == null)
            {
               mediaFile.Cover = GetTrackCover(mediaFile);
                
            }
            else
            {
                try
                {
                    mediaFile.Cover = await BitmapFactory.DecodeByteArrayAsync(imageByteArray, 0, imageByteArray.Length);
                }
                catch (Java.Lang.OutOfMemoryError ex)
                {
                    mediaFile.Cover = null;
                    throw;
                }
            }
            return mediaFile;
        }

        private void SetMetadata(IMediaFile mediaFile, MediaMetadataRetriever retriever)
        {
            mediaFile.Title = retriever?.ExtractMetadata(MetadataKey.Title) ?? "Unknow";
            mediaFile.Artist = retriever?.ExtractMetadata(MetadataKey.Artist) ?? "Unknow";
            mediaFile.Album = retriever?.ExtractMetadata(MetadataKey.Album);
        }

        private async Task<MediaMetadataRetriever> GetMetadataRetriever(IMediaFile currentFile)
        {
            MediaMetadataRetriever metaRetriever = new MediaMetadataRetriever();

            switch (currentFile.Type)
            {
                case MediaFileType.AudioUrl:
                    await metaRetriever.SetDataSourceAsync(currentFile.Url, new Dictionary<string, string>());
                    break;
                case MediaFileType.VideoUrl:
                    break;
                case MediaFileType.AudioFile:
                    Java.IO.File file = new Java.IO.File(currentFile.Url);
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

        private Bitmap GetTrackCover(IMediaFile currentTrack)
        {
            string albumFolder = GetCurrentSongFolder(currentTrack);
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
            return bitmap ?? BitmapFactory.DecodeResource(_resources, Resource.Drawable.IcMediaPlay);
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

        private string GetCurrentSongFolder(IMediaFile currentFile)
        {
            if (!new System.Uri(currentFile.Url).IsFile)
                return null;

            return System.IO.Path.GetDirectoryName(currentFile.Url);
        }
    }
}
