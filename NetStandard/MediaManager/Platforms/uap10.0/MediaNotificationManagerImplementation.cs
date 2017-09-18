using System;
using Windows.Media;
using Windows.Storage;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager
{
    public class MediaNotificationManagerImplementation : IMediaNotificationManager
    {
        private readonly SystemMediaTransportControls _systemMediaTransportControls;

        public MediaNotificationManagerImplementation()
        {
            _systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
        }

        public void StartNotification(IMediaFile mediaFile)
        {
            UpdateInfoFromMediaFile(mediaFile);
        }

        public void StopNotifications()
        {
            _systemMediaTransportControls.DisplayUpdater.ClearAll();
        }

        public void UpdateNotifications(IMediaFile mediaFile, MediaPlayerStatus status)
        {
            UpdateInfoFromMediaFile(mediaFile);
        }

        private async void UpdateInfoFromMediaFile(IMediaFile mediaFile)
        {
            var updater = _systemMediaTransportControls.DisplayUpdater;
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