using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Media;
using MediaManager.Media;

namespace MediaManager.Platforms.Android.Media
{
    public class ID3Provider : IMetadataProvider, IImageProvider, IVideoFrameProvider
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;
        protected Dictionary<string, string> RequestHeaders => MediaManager.RequestHeaders;

        public async Task<IMediaItem> ProvideMetadata(IMediaItem mediaItem)
        {
            try
            {
                var metaRetriever = await CreateMediaRetriever(mediaItem);

                mediaItem = await ExtractMetadata(metaRetriever, mediaItem).ConfigureAwait(false);

                metaRetriever.Release();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return mediaItem;
        }

        protected virtual Task<IMediaItem> ExtractMetadata(MediaMetadataRetriever mediaMetadataRetriever, IMediaItem mediaItem)
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

            return Task.FromResult(mediaItem);
        }

        public async Task<object> ProvideImage(IMediaItem mediaItem)
        {
            Bitmap image = null;
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

        public async Task<object> ProvideVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart)
        {
            Bitmap image = null;
            try
            {
                var metaRetriever = await CreateMediaRetriever(mediaItem);

                image = metaRetriever.GetFrameAtTime((long)timeFromStart.TotalMilliseconds);

                metaRetriever.Release();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return image;
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
