using System;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager
{
    public class MediaNotificationManagerImplementation : INotificationManager
    {
        private readonly SystemMediaTransportControls _systemMediaTransportControls;

        public MediaNotificationManagerImplementation()
        {
            _systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
        }

        public void StartNotification(IMediaItem mediaFile)
        {
            UpdateInfoFromMediaFile(mediaFile);
        }

        public void StopNotifications()
        {
            _systemMediaTransportControls.DisplayUpdater.ClearAll();
        }

        public void UpdateNotifications(IMediaItem mediaFile, MediaPlayerState status)
        {
            UpdateInfoFromMediaFile(mediaFile);
        }

        private async void UpdateInfoFromMediaFile(IMediaItem mediaFile)
        {
            var updater = _systemMediaTransportControls.DisplayUpdater;
            //if (mediaFile.Availability == ResourceAvailability.Local)
            //{
            switch (mediaFile.Type)
            {
                case MediaItemType.Audio:
                    await updater.CopyFromFileAsync(MediaPlaybackType.Music,
                            await StorageFile.GetFileFromPathAsync(mediaFile.Url));
                    break;
                case MediaItemType.Video:
                    await updater.CopyFromFileAsync(MediaPlaybackType.Video,
                            await StorageFile.GetFileFromPathAsync(mediaFile.Url));
                    break;
            }
            //}

            updater.Update();
        }
    }
}