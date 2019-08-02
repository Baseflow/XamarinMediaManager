using System;
using System.Threading.Tasks;
using MediaManager.Media;
using Windows.Media.Editing;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace MediaManager.Platforms.Uap.Media
{
    public class MediaExtractor : MediaExtractorBase, IMediaExtractor
    {
        public MediaExtractor()
        {
        }

        public override async Task<IMediaItem> ExtractMetadata(IMediaItem mediaItem)
        {
            // default title
            mediaItem.Title = System.IO.Path.GetFileNameWithoutExtension(mediaItem.MediaUri);

            if (mediaItem.MediaLocation == MediaLocation.FileSystem)
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

                await SetAlbumArt(file, mediaItem);
            }

            return mediaItem;
        }

        private async Task SetAudioInfo(IStorageItemProperties file, IMediaItem mediaItem)
        {
            var musicProperties = await file.Properties.GetMusicPropertiesAsync();

            mediaItem.Title = musicProperties.Title;
            mediaItem.Artist = musicProperties.Artist;
            mediaItem.Album = musicProperties.Album;
        }

        private async Task SetVideoInfo(IStorageItemProperties file, IMediaItem mediaItem)
        {
            var musicProperties = await file.Properties.GetVideoPropertiesAsync();
            mediaItem.Title = musicProperties.Title;
        }

        private async Task SetAlbumArt(IStorageItemProperties file, IMediaItem mediaItem)
        {
            var isAudio = mediaItem.MediaType == MediaType.Audio;
            var thumbnailMode = isAudio ? ThumbnailMode.MusicView : ThumbnailMode.VideosView;

            var thumbnail = await file.GetThumbnailAsync(thumbnailMode);
            if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
            {
                BitmapSource bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(thumbnail);
                mediaItem.AlbumArt = bitmap;
            }
        }

        public override Task<object> RetrieveMediaItemArt(IMediaItem mediaItem)
        {
            return null;
        }

        public override async Task<object> GetVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart)
        {
            if (mediaItem.MediaLocation == MediaLocation.FileSystem)
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

        public async Task<IInputStream> GetThumbnailAsync(StorageFile file, TimeSpan timeFromStart)
        {
            var mediaClip = await MediaClip.CreateFromFileAsync(file);
            var mediaComposition = new MediaComposition();
            mediaComposition.Clips.Add(mediaClip);
            return await mediaComposition.GetThumbnailAsync(timeFromStart, 0, 0, VideoFramePrecision.NearestFrame);
        }
    }
}
