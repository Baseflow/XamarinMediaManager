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
                switch (mediaFile.Type)
                {
                    case MediaFileType.Audio:
                        await GetAudioInfo(mediaFile);
                        break;

                    case MediaFileType.Video:
                        await GetVideoInfo(mediaFile);
                        break;
                }
            }

            return mediaFile;
        }

        private async Task GetAudioInfo(IMediaFile mediaFile)
        {
            if (mediaFile.Type == MediaFileType.Audio && mediaFile.Availability == ResourceAvailability.Local)
            {
                var file = await StorageFile.GetFileFromPathAsync(mediaFile.Url);
                var musicProperties = await file.Properties.GetMusicPropertiesAsync();
                mediaFile.Metadata.Title = musicProperties.Title;
                mediaFile.Metadata.Artist = musicProperties.Artist;
                mediaFile.Metadata.Album = musicProperties.Album;
                var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView);
                if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
                {
                    BitmapSource bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(thumbnail);
                    mediaFile.Metadata.AlbumArt = bitmap;
                }
            }
            mediaFile.MetadataExtracted = true;
        }

        private async Task GetVideoInfo(IMediaFile mediaFile)
        {
            if (mediaFile.Type == MediaFileType.Video && mediaFile.Availability == ResourceAvailability.Local)
            {
                var file = await StorageFile.GetFileFromPathAsync(mediaFile.Url);
                var musicProperties = await file.Properties.GetVideoPropertiesAsync();
                mediaFile.Metadata.Title = musicProperties.Title;
                mediaFile.Metadata.Album = string.Empty;
                mediaFile.Metadata.Artist = string.Empty;
                var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.VideosView);
                if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
                {
                    BitmapSource bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(thumbnail);
                    mediaFile.Metadata.AlbumArt = bitmap;
                }
            }
            mediaFile.MetadataExtracted = true;
        }
    }
}
