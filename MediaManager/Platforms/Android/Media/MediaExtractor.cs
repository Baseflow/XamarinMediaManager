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
                var metaRetriever = await CreateMediaRetriever(mediaItem);

                mediaItem = await ExtractMediaInfo(metaRetriever, mediaItem).ConfigureAwait(false);

                metaRetriever.Release();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        public override async Task<object> GetMediaItemImage(IMediaItem mediaItem)
        {
            Bitmap image = null;

            if (!string.IsNullOrEmpty(mediaItem.ArtUri))
            {
                try
                {
                    var url = new Java.Net.URL(mediaItem.ArtUri);
                    image = await Task.Run(() => BitmapFactory.DecodeStreamAsync(url.OpenStream()));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                try
                {
                    var metaRetriever = await CreateMediaRetriever(mediaItem);

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
                        image = GetTrackCover(mediaItem);
                    }
                    else
                    {
                        try
                        {
                            image = await BitmapFactory.DecodeByteArrayAsync(imageByteArray, 0, imageByteArray.Length);
                        }
                        catch (Java.Lang.OutOfMemoryError)
                        {
                            mediaItem.AlbumArt = null;
                        }
                    }

                    metaRetriever.Release();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            mediaItem.AlbumArt = image;
            return image;
        }

        public override async Task<object> GetVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart)
        {
            try
            {
                var metaRetriever = await CreateMediaRetriever(mediaItem);

                var bitmap = metaRetriever.GetFrameAtTime((long)timeFromStart.TotalMilliseconds);

                metaRetriever.Release();
                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        protected override async Task<string> GetResourcePath(string resourceName)
        {
            string path = null;

            using (var stream = MediaManager.Context.Assets.Open(resourceName))
            {
                path = await CopyResourceStreamToFile(stream, "AndroidResources", resourceName).ConfigureAwait(false);
            }

            return path;
        }

        protected virtual async Task<MediaMetadataRetriever> CreateMediaRetriever(IMediaItem mediaItem)
        {
            var metaRetriever = new MediaMetadataRetriever();

            if (mediaItem.MediaLocation.IsLocal())
                await metaRetriever.SetDataSourceAsync(mediaItem.MediaUri);
            else
                await metaRetriever.SetDataSourceAsync(mediaItem.MediaUri, RequestHeaders);

            return metaRetriever;
        }
    }
}
