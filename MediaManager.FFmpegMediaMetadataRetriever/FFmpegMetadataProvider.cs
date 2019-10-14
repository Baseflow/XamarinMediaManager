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
                var metadataRetriever = CreateMediaRetriever(mediaItem);

                if (string.IsNullOrEmpty(mediaItem.Album))
                    mediaItem.Album = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyAlbum);

                if (string.IsNullOrEmpty(mediaItem.AlbumArtist))
                    mediaItem.AlbumArtist = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyAlbumArtist);

                if (string.IsNullOrEmpty(mediaItem.Artist))
                    mediaItem.Artist = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyArtist);

                //if (string.IsNullOrEmpty(mediaItem.Author))
                //    mediaItem.Author = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyAuthor);

                var trackNumber = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyTrack);
                if (!string.IsNullOrEmpty(trackNumber) && int.TryParse(trackNumber, out var trackNumberResult))
                    mediaItem.TrackNumber = trackNumberResult;

                //if (string.IsNullOrEmpty(mediaItem.Compilation))
                //    mediaItem.Compilation = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyCompilation);

                if (string.IsNullOrEmpty(mediaItem.Composer))
                    mediaItem.Composer = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyComposer);

                var date = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyDate);
                if (mediaItem.Date == default && !string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var dateResult))
                    mediaItem.Date = dateResult;

                var discNumber = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyDisc);
                if (!string.IsNullOrEmpty(discNumber) && int.TryParse(discNumber, out var discNumberResult))
                    mediaItem.DiscNumber = discNumberResult;

                var duration = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyDuration);
                if (mediaItem.Duration == default && !string.IsNullOrEmpty(duration) && int.TryParse(duration, out var durationResult))
                    mediaItem.Duration = TimeSpan.FromMilliseconds(durationResult);

                if (string.IsNullOrEmpty(mediaItem.Genre))
                    mediaItem.Genre = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyGenre);

                //var numTracks = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyNumTracks);
                //if (!string.IsNullOrEmpty(numTracks) && int.TryParse(numTracks, out var numTracksResult))
                //    mediaItem.NumTracks = numTracksResult;

                if (string.IsNullOrEmpty(mediaItem.Title))
                    mediaItem.Title = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyTitle);

                //if (string.IsNullOrEmpty(mediaItem.Writer))
                //    mediaItem.Writer = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyWriter);

                //var year = metadataRetriever.ExtractMetadata(MediaMetadataRetriever.MetadataKeyYear);
                //if (!string.IsNullOrEmpty(year) && int.TryParse(year, out var yearResult))
                //    mediaItem.Year = yearResult;

                metadataRetriever.Release();
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
                var metadataRetriever = CreateMediaRetriever(mediaItem);
                byte[] imageByteArray = null;
                try
                {
                    imageByteArray = metadataRetriever.GetEmbeddedPicture();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (imageByteArray != null)
                {
                    try
                    {
                        image = await BitmapFactory.DecodeByteArrayAsync(imageByteArray, 0, imageByteArray.Length).ConfigureAwait(false);
                    }
                    catch (Java.Lang.OutOfMemoryError)
                    {
                    }
                }
                metadataRetriever.Release();
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
                var metadataRetriever = CreateMediaRetriever(mediaItem);

                var bitmap = metadataRetriever.GetFrameAtTime((long)timeFromStart.TotalMilliseconds);

                metadataRetriever.Release();
                return Task.FromResult(bitmap as object);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Task.FromResult<object>(null);
        }

        protected virtual MediaMetadataRetriever CreateMediaRetriever(IMediaItem mediaItem)
        {
            var metadataRetriever = new MediaMetadataRetriever();

            if (!mediaItem.MediaLocation.IsLocal() && RequestHeaders.Count > 0)
                metadataRetriever.SetDataSource(mediaItem.MediaUri, RequestHeaders);
            else
                metadataRetriever.SetDataSource(mediaItem.MediaUri);

            return metadataRetriever;
        }
    }
}
