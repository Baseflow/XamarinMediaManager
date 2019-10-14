using System;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Media;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Source.Dash;
using Com.Google.Android.Exoplayer2.Source.Hls;
using Com.Google.Android.Exoplayer2.Source.Smoothstreaming;
using MediaManager.Library;
using MediaManager.Platforms.Android.Player;

namespace MediaManager.Platforms.Android.Media
{
    public static class MediaItemExtensions
    {
        private static MediaManagerImplementation MediaManager => CrossMediaManager.Android;

        public static IMediaSource ToMediaSource(this IMediaItem mediaItem)
        {
            var mediaDescription = mediaItem.ToMediaDescription();
            return ToMediaSource(mediaDescription, mediaItem.MediaType);
        }

        public static ClippingMediaSource ToClippingMediaSource(this IMediaItem mediaItem, TimeSpan stopAt)
        {
            var mediaDescription = mediaItem.ToMediaDescription();
            var mediaSource = ToMediaSource(mediaDescription, mediaItem.MediaType);
            return new ClippingMediaSource(mediaSource, (long)stopAt.TotalMilliseconds * 1000);
        }

        public static ClippingMediaSource ToClippingMediaSource(this IMediaItem mediaItem, TimeSpan startAt, TimeSpan stopAt)
        {
            var mediaDescription = mediaItem.ToMediaDescription();
            var mediaSource = ToMediaSource(mediaDescription, mediaItem.MediaType);
            //Clipping media source takes time values in microseconds
            var startUs = startAt.Ticks / (TimeSpan.TicksPerMillisecond / 1000);
            var endUs = stopAt.Ticks / (TimeSpan.TicksPerMillisecond / 1000);
            return new ClippingMediaSource(mediaSource, startUs, endUs);
        }

        public static IMediaSource ToMediaSource(this MediaDescriptionCompat mediaDescription, MediaType mediaType)
        {
            if (MediaManager.AndroidMediaPlayer.DataSourceFactory == null)
                throw new ArgumentNullException(nameof(AndroidMediaPlayer.DataSourceFactory));

            IMediaSource mediaSource;
            var mediaUri = mediaDescription.MediaUri;

            switch (mediaType)
            {
                case MediaType.Audio:
                case MediaType.Video:
                case MediaType.Default:
                    mediaSource = new ExtractorMediaSource.Factory(MediaManager.AndroidMediaPlayer.DataSourceFactory)
                        .SetTag(mediaDescription)
                        .CreateMediaSource(mediaUri);
                    break;
                case MediaType.Dash:
                    if (MediaManager.AndroidMediaPlayer.DashChunkSourceFactory == null)
                        throw new ArgumentNullException(nameof(AndroidMediaPlayer.DashChunkSourceFactory));

                    mediaSource = new DashMediaSource.Factory(MediaManager.AndroidMediaPlayer.DashChunkSourceFactory, MediaManager.AndroidMediaPlayer.DataSourceFactory)
                        .SetTag(mediaDescription)
                        .CreateMediaSource(mediaUri);
                    break;
                case MediaType.Hls:
                    mediaSource = new HlsMediaSource.Factory(MediaManager.AndroidMediaPlayer.DataSourceFactory)
                        .SetAllowChunklessPreparation(true)
                        .SetTag(mediaDescription)
                        .CreateMediaSource(mediaUri);
                    break;
                case MediaType.SmoothStreaming:
                    if (MediaManager.AndroidMediaPlayer.SsChunkSourceFactory == null)
                        throw new ArgumentNullException(nameof(AndroidMediaPlayer.SsChunkSourceFactory));

                    mediaSource = new SsMediaSource.Factory(MediaManager.AndroidMediaPlayer.SsChunkSourceFactory, MediaManager.AndroidMediaPlayer.DataSourceFactory)
                        .SetTag(mediaDescription)
                        .CreateMediaSource(mediaUri);
                    break;
                default:
                    throw new ArgumentNullException(nameof(mediaType));
            }

            return mediaSource;
        }

        public static MediaDescriptionCompat ToMediaDescription(this IMediaItem item)
        {
            var description = new MediaDescriptionCompat.Builder()
                .SetMediaId(item?.Id)
                .SetMediaUri(global::Android.Net.Uri.Parse(item?.MediaUri))
                .SetTitle(item?.GetTitle())
                .SetSubtitle(item?.GetContentTitle())
                .SetDescription(item?.DisplayDescription)
                .SetExtras(item?.Extras as Bundle);

            //It should be better to only set the uri to prevent loading images into memory
            if (!string.IsNullOrEmpty(item?.GetImageUri()))
                description.SetIconUri(global::Android.Net.Uri.Parse(item?.GetImageUri()));
            else
            {
                var image = item?.GetImage() as Bitmap;
                if (image != null)
                    description.SetIconBitmap(image);
            }

            return description.Build();
        }

        public static MediaBrowserCompat.MediaItem ToMediaBrowserMediaItem(this IMediaItem item)
        {
            var mediaItem = new MediaBrowserCompat.MediaItem(ToMediaDescription(item), MediaBrowserCompat.MediaItem.FlagPlayable);
            return mediaItem;
        }

        public static IMediaItem ToMediaItem(this MediaDescriptionCompat mediaDescription)
        {
            var item = new MediaItem(mediaDescription.MediaUri.ToString());
            //item.Advertisement = mediaDescription.
            //item.Album = mediaDescription.
            //item.AlbumArt = mediaDescription.
            //item.AlbumArtist = mediaDescription.
            //item.AlbumArtUri = mediaDescription.
            //item.Art = mediaDescription.
            //item.Artist = mediaDescription.
            //item.ArtUri = mediaDescription.
            //item.Author = mediaDescription.
            //item.Compilation = mediaDescription.
            //item.Composer = mediaDescription.
            //item.Date = mediaDescription.
            //item.DiscNumber = mediaDescription.
            //item.DisplayDescription = mediaDescription.
            item.DisplayIcon = mediaDescription.IconBitmap;
            item.DisplayIconUri = mediaDescription.IconUri.ToString();
            item.DisplaySubtitle = mediaDescription.Subtitle;
            item.DisplayTitle = mediaDescription.Title;
            //item.DownloadStatus = mediaDescription.
            //item.Duration = mediaDescription.
            item.Extras = mediaDescription.Extras;
            //item.Genre = mediaDescription.
            item.Id = mediaDescription.MediaId;
            //item.MediaUri = mediaDescription.MediaUri.ToString();
            //item.NumTracks = mediaDescription.
            //item.Rating = mediaDescription.
            item.Title = mediaDescription.Title;
            //item.TrackNumber = mediaDescription.
            //item.UserRating = mediaDescription.
            //item.Writer = mediaDescription.
            //item.Year = mediaDescription.
            item.IsMetadataExtracted = true;
            return item;
        }

        public static IMediaItem ToMediaItem(this MediaMetadataCompat mediaMetadata)
        {
            var url = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyMediaUri);
            if (string.IsNullOrEmpty(url))
                return null;

            var item = new MediaItem(url);
            item.Advertisement = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAdvertisement);
            item.Album = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAlbum);
            item.AlbumImage = mediaMetadata.GetBitmap(MediaMetadataCompat.MetadataKeyAlbumArt);
            item.AlbumArtist = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAlbumArtist);
            item.AlbumImageUri = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAlbumArtUri);
            item.Image = mediaMetadata.GetBitmap(MediaMetadataCompat.MetadataKeyArt);
            item.Artist = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyArtist);
            item.ImageUri = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyArtUri);
            item.Author = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAuthor);
            item.Compilation = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyCompilation);
            item.Composer = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyComposer);

            var date = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDate);
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var dateResult))
                item.Date = dateResult;

            item.DiscNumber = Convert.ToInt32(mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyDiscNumber));
            item.DisplayDescription = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDisplayDescription);
            item.DisplayIcon = mediaMetadata.GetBitmap(MediaMetadataCompat.MetadataKeyDisplayIcon);
            item.DisplayIconUri = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDisplayIconUri);
            item.DisplaySubtitle = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDisplaySubtitle);
            item.DisplayTitle = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDisplayTitle);
            item.DownloadStatus = mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyDownloadStatus) == 0 ? DownloadStatus.NotDownloaded : DownloadStatus.Downloaded;
            item.Duration = TimeSpan.FromMilliseconds(Convert.ToInt32(mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyDuration)));
            item.Extras = mediaMetadata.Description?.Extras;
            item.Genre = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyGenre);
            item.Id = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyMediaId);
            item.MediaUri = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyMediaUri);
            item.NumTracks = Convert.ToInt32(mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyNumTracks));
            item.Rating = mediaMetadata.GetRating(MediaMetadataCompat.MetadataKeyRating);
            item.Title = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyTitle);
            item.TrackNumber = Convert.ToInt32(mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyTrackNumber));
            item.UserRating = mediaMetadata.GetRating(MediaMetadataCompat.MetadataKeyUserRating);
            item.Writer = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyWriter);
            item.Year = Convert.ToInt32(mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyYear));
            item.IsMetadataExtracted = true;
            return item;
        }
    }
}
