using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Graphics;
using MediaManager.Media;
using MediaManager.Platforms.Android.Media;
using MediaMetadataRetriever = Wseemann.Media.FFmpegMediaMetadataRetriever;

namespace MediaManager.FFmpegMediaMetadataRetriever
{
    public class FFmpegMediaExtractor : MediaExtractor
    {
        public override async Task<IMediaItem> ExtractMetadata(IMediaItem mediaItem)
        {
            try
            {
                var metaRetriever = new MediaMetadataRetriever();

                if (mediaItem.MediaLocation.IsLocal())
                    metaRetriever.SetDataSource(mediaItem.MediaUri);
                else
                    metaRetriever.SetDataSource(mediaItem.MediaUri, RequestHeaders);

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
                    mediaItem.AlbumArt = GetTrackCover(mediaItem);
                }
                else
                {
                    try
                    {
                        mediaItem.AlbumArt = await BitmapFactory.DecodeByteArrayAsync(imageByteArray, 0, imageByteArray.Length);
                    }
                    catch (Java.Lang.OutOfMemoryError)
                    {
                        mediaItem.AlbumArt = null;
                    }
                }

                mediaItem.IsMetadataExtracted = true;
                //TODO: Should we call metaRetriever.Release(); ?
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return mediaItem;
        }

        public override Task<object> GetVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart)
        {
            try
            {
                var metaRetriever = new MediaMetadataRetriever();

                if (mediaItem.MediaLocation.IsLocal())
                    metaRetriever.SetDataSource(mediaItem.MediaUri);
                else
                    metaRetriever.SetDataSource(mediaItem.MediaUri, RequestHeaders);

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
    }
}
