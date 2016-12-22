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
        private async Task<IMediaFile> GetAudioInfo(IMediaFile mediaFile)
        {
            if (mediaFile.Type == MediaFileType.AudioFile)
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
            return mediaFile;
        }

        private async Task<IMediaFile> GetVideoInfo(IMediaFile mediaFile)
        {
            if (mediaFile.Type == MediaFileType.VideoFile)
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
            return mediaFile;
        }

        public async Task<IMediaFile> ExtractMediaInfo(IMediaFile mediaFile)
        {
            switch (mediaFile.Type)
            {
                case MediaFileType.AudioUrl:
                case MediaFileType.AudioFile:
                    await GetAudioInfo(mediaFile);
                    return mediaFile;
                case MediaFileType.VideoUrl:
                case MediaFileType.VideoFile:
                    return await GetVideoInfo(mediaFile);
                case MediaFileType.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return await Task.FromResult(mediaFile);
        }
    }
}
