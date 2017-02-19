using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.Content.Res;
using Android.Graphics;
using Android.Media;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class MediaExtractorImplementation : IMediaExtractor
    {
        private readonly Resources _resources;
        private readonly Dictionary<string,string> _requestHeaders;

        public MediaExtractorImplementation(Resources resources, Dictionary<string, string> requestHeaders)
        {
            _resources = resources;
            _requestHeaders = requestHeaders;
        }

        public async Task<IMediaFile> ExtractMediaInfo(IMediaFile mediaFile)
        {
            if (mediaFile.MetadataExtracted) return mediaFile;
            MediaMetadataRetriever metaRetriever = await GetMetadataRetriever(mediaFile);
            SetMetadata(mediaFile, metaRetriever);
            byte[] imageByteArray = null;
            try
            {
                imageByteArray = metaRetriever.GetEmbeddedPicture();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            if (imageByteArray == null)
            {
                mediaFile.Metadata.AlbumArt = GetTrackCover(mediaFile);
            }
            else
            {
                try
                {
                    mediaFile.Metadata.AlbumArt = await BitmapFactory.DecodeByteArrayAsync(imageByteArray, 0, imageByteArray.Length);
                }
                catch (Java.Lang.OutOfMemoryError)
                {
                    mediaFile.Metadata.AlbumArt = null;
                }
            }
            mediaFile.MetadataExtracted = true;
            return mediaFile;
        }

        private void SetMetadata(IMediaFile mediaFile, MediaMetadataRetriever retriever)
        {
            mediaFile.Metadata.Title = retriever?.ExtractMetadata(MetadataKey.Title) ?? "Unknown";
            mediaFile.Metadata.Artist = retriever?.ExtractMetadata(MetadataKey.Artist) ?? "Unknown";
            mediaFile.Metadata.Album = retriever?.ExtractMetadata(MetadataKey.Album);
        }

        private async Task<MediaMetadataRetriever> GetMetadataRetriever(IMediaFile currentFile)
        {
            var metaRetriever = new MediaMetadataRetriever();

            if (currentFile.Type == MediaFileType.Audio)
            {
                if (currentFile.Availability == ResourceAvailability.Remote)
                {
                    await metaRetriever.SetDataSourceAsync(currentFile.Url, _requestHeaders);
                }
                else
                {
                    var file = new Java.IO.File(currentFile.Url);
                    var inputStream = new Java.IO.FileInputStream(file);
                    await metaRetriever.SetDataSourceAsync(inputStream.FD);
                }
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
