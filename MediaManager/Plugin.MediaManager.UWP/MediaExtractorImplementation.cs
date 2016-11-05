using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class MediaExtractorImplementation : IMediaExtractor
    {
        public async Task<IMediaFile> GetAudioInfo(IMediaFile mediaFile)
        {
            var source = MediaSource.CreateFromUri(new Uri(mediaFile.Url));
            var playbackItem = new MediaPlaybackItem(source);
            var displayProperties = playbackItem.GetDisplayProperties();
            var props = displayProperties.MusicProperties;
            mediaFile.Metadata.Title = props.Title;
            mediaFile.Metadata.Artist = props.Artist;
            mediaFile.Metadata.Album = props.AlbumTitle;
            mediaFile.Metadata.Cover = displayProperties.Thumbnail;
            return mediaFile;
        }

        public async Task<IMediaFile> GetVideoInfo(IMediaFile mediaFile)
        {
            var source = MediaSource.CreateFromUri(new Uri(mediaFile.Url));
            var playbackItem = new MediaPlaybackItem(source);
            var displayProperties = playbackItem.GetDisplayProperties();
            var props = displayProperties.VideoProperties;
            mediaFile.Metadata.Title = props.Title;
            mediaFile.Metadata.Cover = displayProperties.Thumbnail;
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
