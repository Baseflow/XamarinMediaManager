using System;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Interfaces;

namespace Plugin.MediaManager
{
    public class BasePlayerImplementation : IDisposable
    {
        private readonly IMediaPlyerPlaybackController mediaPlyerPlaybackController;

        protected readonly MediaPlayer Player;

        protected readonly MediaPlaybackList PlaybackList = new MediaPlaybackList();

        public BasePlayerImplementation(IMediaPlyerPlaybackController mediaPlyerPlaybackController)
        {
            this.mediaPlyerPlaybackController = mediaPlyerPlaybackController;

            Player = mediaPlyerPlaybackController.Player;
        }

        public void Dispose()
        {
            mediaPlyerPlaybackController?.Dispose();
        }

        protected async Task<MediaPlaybackItem> CreateMediaPlaybackItem(IMediaFile mediaFile)
        {
            if (string.IsNullOrWhiteSpace(mediaFile?.Url))
            {
                return null;
            }

            var mediaSource = await CreateMediaSource(mediaFile);
            if (mediaSource == null)
            {
                return null;
            }

            return new MediaPlaybackItem(mediaSource);
        }

        protected async Task<MediaSource> CreateMediaSource(IMediaFile mediaFile)
        {
            switch (mediaFile.Availability)
            {
                case ResourceAvailability.Remote:
                    return MediaSource.CreateFromUri(new Uri(mediaFile.Url));
                case ResourceAvailability.Local:
                    var du = Player.SystemMediaTransportControls.DisplayUpdater;
                    var storageFile = await StorageFile.GetFileFromPathAsync(mediaFile.Url);
                    var playbackType = mediaFile.Type == MediaFileType.Audio
                        ? MediaPlaybackType.Music
                        : MediaPlaybackType.Video;
                    await du.CopyFromFileAsync(playbackType, storageFile);
                    du.Update();
                    return MediaSource.CreateFromStorageFile(storageFile);
            }

            return MediaSource.CreateFromUri(new Uri(mediaFile.Url));
        }
    }
}
