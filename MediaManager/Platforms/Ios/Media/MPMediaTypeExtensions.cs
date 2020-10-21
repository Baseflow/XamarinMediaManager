using MediaManager.Library;
using MediaPlayer;

namespace MediaManager.Platforms.Ios.Media
{
    public static class MPMediaTypeExtensions
    {
        public static MediaType ToMediaType(this MPMediaType mediaType)
        {
            switch (mediaType)
            {
                case MPMediaType.Music:
                case MPMediaType.Podcast:
                case MPMediaType.AudioBook:
                case MPMediaType.AudioITunesU:
                case MPMediaType.AnyAudio:
                    return MediaType.Audio;
                case MPMediaType.Movie:
                case MPMediaType.TVShow:
                case MPMediaType.VideoPodcast:
                case MPMediaType.MusicVideo:
                case MPMediaType.VideoITunesU:
                case MPMediaType.HomeVideo:
                case MPMediaType.TypeAnyVideo:
                    return MediaType.Video;
                case MPMediaType.Any:
                default:
                    return MediaType.Default;
            }
        }
    }
}
