using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using MediaManager.Library;
using MediaManager.Media;
using MediaMetadataRetriever = Wseemann.Media.FFmpegMediaMetadataRetriever;

namespace MediaManager.FFmpegMediaMetadataRetriever
{
    public class FFmpegMetadataProvider : IMediaItemMetadataProvider, IMediaItemImageProvider, IMediaItemVideoFrameProvider
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;
        protected Dictionary<string, string> RequestHeaders => MediaManager.RequestHeaders;

        public Task<IMediaItem> ProvideMetadata(IMediaItem mediaItem)
        {
            try
            {
                var metaRetriever = CreateMediaRetriever(mediaItem);

                if (string.IsNullOrEmpty(mediaItem.Album))
                    mediaItem.Album = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyAlbum);

                if (string.IsNullOrEmpty(mediaItem.AlbumArtist))
                    mediaItem.AlbumArtist = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyAlbumArtist);

                if (string.IsNullOrEmpty(mediaItem.Artist))
                    mediaItem.Artist = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyArtist);

                //if (string.IsNullOrEmpty(mediaItem.Author))
                //    mediaItem.Author = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyAuthor);

                var trackNumber = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyTrack);
                if (!string.IsNullOrEmpty(trackNumber) && int.TryParse(trackNumber, out var trackNumberResult))
                    mediaItem.TrackNumber = trackNumberResult;

                //if (string.IsNullOrEmpty(mediaItem.Compilation))
                //    mediaItem.Compilation = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyCompilation);

                if (string.IsNullOrEmpty(mediaItem.Composer))
                    mediaItem.Composer = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyComposer);

                if (string.IsNullOrEmpty(mediaItem.Date))
                    mediaItem.Date = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyDate);

                var discNumber = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyDisc);
                if (!string.IsNullOrEmpty(discNumber) && int.TryParse(discNumber, out var discNumberResult))
                    mediaItem.DiscNumber = discNumberResult;

                var duration = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyDuration);
                if (!string.IsNullOrEmpty(duration) && int.TryParse(duration, out var durationResult))
                    mediaItem.Duration = TimeSpan.FromMilliseconds(durationResult);

                if (string.IsNullOrEmpty(mediaItem.Genre))
                    mediaItem.Genre = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyGenre);

                //var numTracks = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyNumTracks);
                //if (!string.IsNullOrEmpty(numTracks) && int.TryParse(numTracks, out var numTracksResult))
                //    mediaItem.NumTracks = numTracksResult;

                if (string.IsNullOrEmpty(mediaItem.Title))
                    mediaItem.Title = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyTitle);

                //if (string.IsNullOrEmpty(mediaItem.Writer))
                //    mediaItem.Writer = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyWriter);

                //var year = metaRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyYear);
                //if (!string.IsNullOrEmpty(year) && int.TryParse(year, out var yearResult))
                //    mediaItem.Year = yearResult;

                mediaItem.IsMetadataExtracted = true;
                metaRetriever.Release();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Task.FromResult(mediaItem);
        }

        public async Task<object> ProvideImage(IMediaItem mediaItem)
        {
            object image = null;
            try
            {
                var metaRetriever = CreateMediaRetriever(mediaItem);
                byte[] imageByteArray = null;
                try
                {
                    imageByteArray = metaRetriever.GetEmbeddedPicture();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (imageByteArray != null)
                {
                    try
                    {
                        image = await BitmapFactory.DecodeByteArrayAsync(imageByteArray, 0, imageByteArray.Length);
                    }
                    catch (Java.Lang.OutOfMemoryError)
                    {
                    }
                }
                metaRetriever.Release();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return image;
        }

        public Task<object> ProvideVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart)
        {
            try
            {
                var metaRetriever = CreateMediaRetriever(mediaItem);

                var bitmap = metaRetriever.GetFrameAtTime((long)timeFromStart.TotalMilliseconds);

                metaRetriever.Release();
                return Task.FromResult(bitmap as object);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        protected virtual MediaMetadataRetriever CreateMediaRetriever(IMediaItem mediaItem)
        {
            var metaRetriever = new MediaMetadataRetriever();

            if (!mediaItem.MediaLocation.IsLocal() && RequestHeaders.Count > 0)
                metaRetriever.SetDataSource(mediaItem.MediaUri, RequestHeaders);
            else
                metaRetriever.SetDataSource(mediaItem.MediaUri);

            return metaRetriever;
        }
    }
}
