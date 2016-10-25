using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class MediaExtractorImplementation : IMediaExtractor
    {
        public async Task<IAudioInfo> GetAudioInfo(IMediaFile mediaFile)
        {
            var source = MediaSource.CreateFromUri(new Uri(mediaFile.Url));
            var playbackItem = new MediaPlaybackItem(source);
            var displayProperties = playbackItem.GetDisplayProperties();
            var stream = await displayProperties.Thumbnail.OpenReadAsync();

            AudioInfo info = new AudioInfo();
            var props = displayProperties.MusicProperties;
            info.AlbumArtist = props.AlbumArtist;
            info.AlbumTitle = props.AlbumTitle;
            info.AlbumTrackCount = (int) props.AlbumTrackCount;
            info.Artist = props.Title;
            info.Genres = props.Genres;
            info.TrackNumber = (int) props.TrackNumber;
            return info;
        }

        public async Task<IMediaFile> GetVideoInfo(IMediaFile mediaFile)
        {
            var source = MediaSource.CreateFromUri(new Uri(mediaFile.Url));
            var playbackItem = new MediaPlaybackItem(source);
            var displayProperties = playbackItem.GetDisplayProperties();
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

    public class AudioInfo : IAudioInfo
    {
        
        public string AlbumArtist { get; set; }
        public string AlbumTitle { get; set; }
        public int AlbumTrackCount { get; set; }
        public string Artist { get; set; }
        public IList<string> Genres { get; set; }
        public string Title { get; set; }
        public int TrackNumber { get; set; }
        public object Cover { get; set; }
    }

    public interface IAudioInfo
    {
        string AlbumArtist { get; set; }
        string AlbumTitle { get; set; }
        int AlbumTrackCount { get; set; }
        string Artist { get; set; }
        IList<string> Genres { get; }
        string Title { get; set; }
        int TrackNumber { get; set; }
    }
}
