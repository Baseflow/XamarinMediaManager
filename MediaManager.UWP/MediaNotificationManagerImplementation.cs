using System;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Interfaces;

namespace Plugin.MediaManager
{
    public class MediaNotificationManagerImplementation : IMediaNotificationManager
    {
        private readonly IMediaPlyerPlaybackController _mediaPlyerPlaybackController;

        public MediaNotificationManagerImplementation(IMediaPlyerPlaybackController mediaPlyerPlaybackController)
        {
            _mediaPlyerPlaybackController = mediaPlyerPlaybackController;
        }

        public void StartNotification(IMediaFile mediaFile)
        {
            if (mediaFile == null || _mediaPlyerPlaybackController?.Player == null)
            {
                return;
            }

            if (_mediaPlyerPlaybackController.Player.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
            {
                _mediaPlyerPlaybackController.Player.Play();
            }

            UpdateInfoFromMediaFile(mediaFile);
        }

        public void StopNotifications()
        {
            _mediaPlyerPlaybackController?.Player?.SystemMediaTransportControls.DisplayUpdater.ClearAll();
        }

        public void UpdateNotifications(IMediaFile mediaFile, MediaPlayerStatus status)
        {
            if (mediaFile == null || _mediaPlyerPlaybackController?.Player == null)
            {
                return;
            }

            switch (status)
            {
                case MediaPlayerStatus.Stopped:
                case MediaPlayerStatus.Paused:
                    _mediaPlyerPlaybackController.Player.Pause();
                    break;
                case MediaPlayerStatus.Playing:
                    _mediaPlyerPlaybackController.Player.Play();
                    break;
            }


            UpdateInfoFromMediaFile(mediaFile);
        }

        private async void UpdateInfoFromMediaFile(IMediaFile mediaFile)
        {
            if (mediaFile == null || _mediaPlyerPlaybackController?.Player == null)
            {
                return;
            }

            var updater = _mediaPlyerPlaybackController.Player.SystemMediaTransportControls.DisplayUpdater;
            if (mediaFile.Availability == ResourceAvailability.Local)
            {
                switch (mediaFile.Type)
                {
                    case MediaFileType.Audio:
                        await updater.CopyFromFileAsync(MediaPlaybackType.Music,
                                await StorageFile.GetFileFromPathAsync(mediaFile.Url));
                        break;
                    case MediaFileType.Video:
                        await updater.CopyFromFileAsync(MediaPlaybackType.Video,
                                await StorageFile.GetFileFromPathAsync(mediaFile.Url));
                        break;
                }
            }

            updater.Update();
        }
    }
}