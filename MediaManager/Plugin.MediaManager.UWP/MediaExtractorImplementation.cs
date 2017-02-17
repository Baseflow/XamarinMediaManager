using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager
{
    public class MediaExtractorImplementation : IMediaExtractor
    {
        public async Task<IMediaFile> ExtractMediaInfo(IMediaFile mediaFile)
        {
            if (mediaFile.Availability == ResourceAvailability.Local)
            {
                var file = await StorageFile.GetFileFromPathAsync(mediaFile.Url);

                switch (mediaFile.Type)
                {
                    case MediaFileType.Audio:
                        await SetAudioInfo(file, mediaFile);
                        break;

                    case MediaFileType.Video:
                        await SetVideoInfo(file, mediaFile);
                        break;
                }

                await SetAlbumArt(file, mediaFile);
            }

            mediaFile.MetadataExtracted = true;

            return mediaFile;
        }

        private async Task SetAudioInfo(IStorageItemProperties file, IMediaFile mediaFile)
        {;
            var musicProperties = await file.Properties.GetMusicPropertiesAsync();
            mediaFile.Metadata.Title = musicProperties.Title;
            mediaFile.Metadata.Artist = musicProperties.Artist;
            mediaFile.Metadata.Album = musicProperties.Album;
        }

        private async Task SetVideoInfo(IStorageItemProperties file, IMediaFile mediaFile)
        {
            var musicProperties = await file.Properties.GetVideoPropertiesAsync();
            mediaFile.Metadata.Title = musicProperties.Title;
        }

        private async Task SetAlbumArt(IStorageItemProperties file, IMediaFile mediaFile)
        {
            var isAudio = mediaFile.Type == MediaFileType.Audio;
            var thumbnailMode = isAudio ? ThumbnailMode.MusicView : ThumbnailMode.VideosView;

            var thumbnail = await file.GetThumbnailAsync(thumbnailMode);
            if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
            {
                BitmapSource bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(thumbnail);
                mediaFile.Metadata.AlbumArt = bitmap;
            }
        }
    }
}
