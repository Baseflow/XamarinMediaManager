using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Media;
using MediaManager.Library;
using MediaManager.Media;

namespace MediaManager.Platforms.Android.Media
{
    public class ID3Provider : IMediaItemMetadataProvider, IMediaItemImageProvider, IMediaItemVideoFrameProvider
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;
        protected Dictionary<string, string> RequestHeaders => MediaManager.RequestHeaders;

        public async Task<IMediaItem> ProvideMetadata(IMediaItem mediaItem)
        {
            try
            {
                var metadataRetriever = await CreateMediaRetriever(mediaItem).ConfigureAwait(false);

                mediaItem = await ExtractMetadata(metadataRetriever, mediaItem).ConfigureAwait(false);

                metadataRetriever.Release();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return mediaItem;
        }

        protected virtual Task<IMediaItem> ExtractMetadata(MediaMetadataRetriever metadataRetriever, IMediaItem mediaItem)
        {
            if (metadataRetriever == null)
                return Task.FromResult(mediaItem);

            if (string.IsNullOrEmpty(mediaItem.Album))
                mediaItem.Album = metadataRetriever.ExtractMetadata(MetadataKey.Album);

            if (string.IsNullOrEmpty(mediaItem.AlbumArtist))
                mediaItem.AlbumArtist = metadataRetriever.ExtractMetadata(MetadataKey.Albumartist);

            if (string.IsNullOrEmpty(mediaItem.Artist))
                mediaItem.Artist = metadataRetriever.ExtractMetadata(MetadataKey.Artist);

            if (string.IsNullOrEmpty(mediaItem.Author))
                mediaItem.Author = metadataRetriever.ExtractMetadata(MetadataKey.Author);

            var trackNumber = metadataRetriever.ExtractMetadata(MetadataKey.CdTrackNumber);
            if (!string.IsNullOrEmpty(trackNumber) && int.TryParse(trackNumber, out var trackNumberResult))
                mediaItem.TrackNumber = trackNumberResult;

            if (string.IsNullOrEmpty(mediaItem.Compilation))
                mediaItem.Compilation = metadataRetriever.ExtractMetadata(MetadataKey.Compilation);

            if (string.IsNullOrEmpty(mediaItem.Composer))
                mediaItem.Composer = metadataRetriever.ExtractMetadata(MetadataKey.Composer);

            var date = metadataRetriever.ExtractMetadata(MetadataKey.Date);
            if (mediaItem.Date == default && !string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var dateResult))
                mediaItem.Date = dateResult;

            var discNumber = metadataRetriever.ExtractMetadata(MetadataKey.DiscNumber);
            if (!string.IsNullOrEmpty(discNumber) && int.TryParse(discNumber, out var discNumberResult))
                mediaItem.DiscNumber = discNumberResult;

            var duration = metadataRetriever.ExtractMetadata(MetadataKey.Duration);
            if (mediaItem.Duration == default && !string.IsNullOrEmpty(duration) && int.TryParse(duration, out var durationResult))
                mediaItem.Duration = TimeSpan.FromMilliseconds(durationResult);

            if (string.IsNullOrEmpty(mediaItem.Genre))
                mediaItem.Genre = metadataRetriever.ExtractMetadata(MetadataKey.Genre);

            var numTracks = metadataRetriever.ExtractMetadata(MetadataKey.NumTracks);
            if (!string.IsNullOrEmpty(numTracks) && int.TryParse(numTracks, out var numTracksResult))
                mediaItem.NumTracks = numTracksResult;

            if (string.IsNullOrEmpty(mediaItem.Title))
                mediaItem.Title = metadataRetriever.ExtractMetadata(MetadataKey.Title);

            if (string.IsNullOrEmpty(mediaItem.Writer))
                mediaItem.Writer = metadataRetriever.ExtractMetadata(MetadataKey.Writer);

            var year = metadataRetriever.ExtractMetadata(MetadataKey.Year);
            if (!string.IsNullOrEmpty(year) && int.TryParse(year, out var yearResult))
                mediaItem.Year = yearResult;

            return Task.FromResult(mediaItem);
        }

        public async Task<object> ProvideImage(IMediaItem mediaItem)
        {
            Bitmap image = null;
            try
            {
                var metadataRetriever = await CreateMediaRetriever(mediaItem).ConfigureAwait(false);

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
            return mediaItem.Image = image;
        }

        public async Task<object> ProvideVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart)
        {
            Bitmap image = null;
            try
            {
                var metadataRetriever = await CreateMediaRetriever(mediaItem).ConfigureAwait(false);

                image = metadataRetriever.GetFrameAtTime((long)timeFromStart.TotalMilliseconds);

                metadataRetriever.Release();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return image;
        }

        protected virtual async Task<MediaMetadataRetriever> CreateMediaRetriever(IMediaItem mediaItem)
        {
            var metadataRetriever = new MediaMetadataRetriever();
            try
            {
                if (mediaItem.MediaLocation.IsLocal())
                    await metadataRetriever.SetDataSourceAsync(mediaItem.MediaUri).ConfigureAwait(false);
                else
                    await metadataRetriever.SetDataSourceAsync(mediaItem.MediaUri, RequestHeaders).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return metadataRetriever;
        }
    }
}
