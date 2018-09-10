using System;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Media;
using MediaManager.Media;

namespace MediaManager
{
    public static class MediaItemExtensions
    {
        public static MediaDescriptionCompat ToMediaDescription(this IMediaItem item)
        {
            var description = new MediaDescriptionCompat.Builder()
                .SetMediaId(item?.MediaId)
                .SetMediaUri(Android.Net.Uri.Parse(item?.MediaUri))
                .SetTitle(item?.Title)
                .SetSubtitle(item?.Artist)
                .SetDescription(item?.DisplayDescription)
                .SetExtras(item?.Extras as Bundle)
                .SetIconBitmap(item?.AlbumArt as Bitmap)
                .SetIconUri(item?.DisplayIconUri != null ? Android.Net.Uri.Parse(item?.DisplayIconUri) : null)
                .Build();

            return description;
        }

        public static MediaBrowserCompat.MediaItem ToMediaBrowserMediaItem(this IMediaItem item)
        {
            var media = new MediaBrowserCompat.MediaItem(ToMediaDescription(item), MediaBrowserCompat.MediaItem.FlagPlayable);
            return media;
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
            //item.BtFolderType = mediaDescription.
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
            item.MediaId = mediaDescription.MediaId;
            item.MediaUri = mediaDescription.MediaUri.ToString();
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
            var item = new MediaItem(mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyMediaUri));
            item.Advertisement = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAdvertisement);
            item.Album = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAlbum);
            item.AlbumArt = mediaMetadata.GetBitmap(MediaMetadataCompat.MetadataKeyAlbumArt);
            item.AlbumArtist = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAlbumArtist);
            item.AlbumArtUri = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAlbumArtUri);
            item.Art = mediaMetadata.GetBitmap(MediaMetadataCompat.MetadataKeyArt);
            item.Artist = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyArtist);
            item.ArtUri = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyArtUri);
            item.Author = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAuthor);
            //item.BtFolderType = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyBtFolderType);
            item.Compilation = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyCompilation);
            item.Composer = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyComposer);
            item.Date = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDate);
            item.DiscNumber = Convert.ToInt32(mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyDiscNumber));
            item.DisplayDescription = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDisplayDescription);
            item.DisplayIcon = mediaMetadata.GetBitmap(MediaMetadataCompat.MetadataKeyDisplayIcon);
            item.DisplayIconUri = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDisplayIconUri);
            item.DisplaySubtitle = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDisplaySubtitle);
            item.DisplayTitle = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDisplayTitle);
            item.DownloadStatus = mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyDownloadStatus) == 0 ? DownloadStatus.NotDownloaded : DownloadStatus.Downloaded;
            item.Duration = TimeSpan.FromMilliseconds(Convert.ToInt32(mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyDuration)));
            //item.Extras = mediaMetadata.GetString(MediaMetadataCompat.extr);
            item.Genre = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyGenre);
            item.MediaId = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyMediaId);
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
