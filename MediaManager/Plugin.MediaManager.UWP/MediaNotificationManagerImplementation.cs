using System;
using Windows.Media;
using Windows.Storage.Streams;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class MediaNotificationManagerImplementation : IMediaNotificationManager
    {
        private readonly SystemMediaTransportControls _systemMediaControls;

        public MediaNotificationManagerImplementation()
        {
            _systemMediaControls = SystemMediaTransportControls.GetForCurrentView();
        }
        public void StartNotification(IMediaFile mediaFile)
        {
            UpdateInfoFromMediaFile(mediaFile);
        }

        private void UpdateInfoFromMediaFile(IMediaFile mediaFile)
        {
            var updater = _systemMediaControls.DisplayUpdater;
            switch (mediaFile.Type)
            {
                case MediaFileType.AudioUrl:
                case MediaFileType.AudioFile:
                    updater.Type = MediaPlaybackType.Music;
                    break;
                case MediaFileType.VideoUrl:
                case MediaFileType.VideoFile:
                    updater.Type = MediaPlaybackType.Video;
                    break;
                case MediaFileType.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            updater.MusicProperties.Artist = mediaFile.Metadata.Artist;
            updater.MusicProperties.AlbumTitle = mediaFile.Metadata.Album;
            updater.MusicProperties.Title = mediaFile.Metadata.Title;
            updater.Thumbnail = (RandomAccessStreamReference) mediaFile.Metadata.Cover;
        }

        public void StopNotifications()
        {
            _systemMediaControls.DisplayUpdater.ClearAll();
        }

        public void UpdateNotifications(IMediaFile mediaFile, MediaPlayerStatus status)
        {
            switch (status)
            {
                case MediaPlayerStatus.Stopped:
                    _systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    break;
                case MediaPlayerStatus.Paused:
                    _systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case MediaPlayerStatus.Playing:
                    _systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case MediaPlayerStatus.Loading:
                    _systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    break;
                case MediaPlayerStatus.Buffering:
                    _systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    break;
                case MediaPlayerStatus.Failed:
                    _systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
            UpdateInfoFromMediaFile(mediaFile);
        }
    }
}

