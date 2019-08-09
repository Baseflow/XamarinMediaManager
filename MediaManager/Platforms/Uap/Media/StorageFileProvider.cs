using System;
using System.Threading.Tasks;
using MediaManager.Library;
using MediaManager.Media;
using Windows.Media.Editing;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace MediaManager.Platforms.Uap.Media
{
    public class StorageFileProvider : IMediaItemMetadataProvider, IMediaItemImageProvider, IMediaItemVideoFrameProvider
    {
        public async Task<IMediaItem> ProvideMetadata(IMediaItem mediaItem)
        {
            if (mediaItem.MediaLocation.IsLocal())
            {
                var file = await StorageFile.GetFileFromPathAsync(mediaItem.MediaUri);

                switch (mediaItem.MediaType)
                {
                    case MediaType.Audio:
                        await SetAudioInfo(file, mediaItem);
                        break;

                    case MediaType.Video:
                        await SetVideoInfo(file, mediaItem);
                        break;
                }
            }
            return mediaItem;
        }

        protected async Task SetAudioInfo(IStorageItemProperties file, IMediaItem mediaItem)
        {
            var musicProperties = await file.Properties.GetMusicPropertiesAsync();

            mediaItem.Album = musicProperties.Album;
            mediaItem.AlbumArtist = musicProperties.AlbumArtist;
            mediaItem.Artist = musicProperties.Artist;
            //mediaItem.Composer = musicProperties.Composers;
            mediaItem.Duration = musicProperties.Duration;
            //mediaItem.Genre = musicProperties.Genre;
            mediaItem.Rating = musicProperties.Rating;
            mediaItem.DisplaySubtitle = musicProperties.Subtitle;
            mediaItem.Title = musicProperties.Title;
            mediaItem.TrackNumber = (int)musicProperties.TrackNumber;
            //mediaItem.Writer = musicProperties.Writers;
            //mediaItem.Year = musicProperties.Year;
        }

        protected async Task SetVideoInfo(IStorageItemProperties file, IMediaItem mediaItem)
        {
            var musicProperties = await file.Properties.GetVideoPropertiesAsync();
            mediaItem.Duration = musicProperties.Duration;
            mediaItem.Rating = musicProperties.Rating;
            mediaItem.DisplaySubtitle = musicProperties.Subtitle;
            mediaItem.Title = musicProperties.Title;
            //mediaItem.Year = musicProperties.Year;
        }

        public async Task<object> ProvideImage(IMediaItem mediaItem)
        {
            object image = null;
            if (mediaItem.MediaLocation.IsLocal())
            {
                var file = await StorageFile.GetFileFromPathAsync(mediaItem.MediaUri);

                var isAudio = mediaItem.MediaType == MediaType.Audio;
                var thumbnailMode = isAudio ? ThumbnailMode.MusicView : ThumbnailMode.VideosView;

                var thumbnail = await file.GetThumbnailAsync(thumbnailMode);
                if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
                {
                    BitmapSource bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(thumbnail);
                    image = bitmap;
                }
            }
            return image;
        }

        public async Task<object> ProvideVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart)
        {
            if (mediaItem.MediaLocation.IsLocal())
            {
                var file = await StorageFile.GetFileFromPathAsync(mediaItem.MediaUri);
                var thumbnail = await GetThumbnailAsync(file, timeFromStart);

                BitmapImage bitmapImage = new BitmapImage();
                InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
                await RandomAccessStream.CopyAsync(thumbnail, randomAccessStream);
                randomAccessStream.Seek(0);
                bitmapImage.SetSource(randomAccessStream);
                return bitmapImage;
            }
            return null;
        }

        protected async Task<IInputStream> GetThumbnailAsync(StorageFile file, TimeSpan timeFromStart)
        {
            var mediaClip = await MediaClip.CreateFromFileAsync(file);
            var mediaComposition = new MediaComposition();
            mediaComposition.Clips.Add(mediaClip);
            return await mediaComposition.GetThumbnailAsync(timeFromStart, 0, 0, VideoFramePrecision.NearestFrame);
        }
    }
}
