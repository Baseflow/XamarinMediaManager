using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;
using Plugin.MediaManager.Interfaces;

namespace Plugin.MediaManager
{
    public class BasePlayerImplementation : IDisposable
    {
        private readonly IMediaQueue _mediaQueue;
        private readonly IMediaPlyerPlaybackController _mediaPlyerPlaybackController;
        private readonly IVolumeManager _volumeManager;

        protected readonly MediaPlayer Player;

        protected readonly MediaPlaybackList PlaybackList = new MediaPlaybackList();

        public BasePlayerImplementation(IMediaQueue mediaQueue, IMediaPlyerPlaybackController mediaPlyerPlaybackController, IVolumeManager volumeManager)
        {
            _mediaPlyerPlaybackController = mediaPlyerPlaybackController;
            _mediaQueue = mediaQueue;
            _volumeManager = volumeManager;

            Player = _mediaPlyerPlaybackController.Player;

            _mediaQueue.CollectionChanged += MediaQueueCollectionChanged;

            _volumeManager.CurrentVolume = (int)Player.Volume * 100;
            _volumeManager.Muted = Player.IsMuted;
            _volumeManager.VolumeChanged += VolumeChanged;
        }

        public void Dispose()
        {
            _mediaQueue.CollectionChanged -= MediaQueueCollectionChanged;
            _volumeManager.VolumeChanged -= VolumeChanged;
            _mediaPlyerPlaybackController?.Dispose();
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

        private async void MediaQueueCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    await HandleMediaQueueAddAction(e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    PlaybackList.Items.Clear();
                    break;
            }
        }

        private void VolumeChanged(object sender, VolumeChangedEventArgs volumeChangedEventArgs)
        {
            Player.Volume = (double)volumeChangedEventArgs.NewVolume;
            Player.IsMuted = volumeChangedEventArgs.Muted;
        }

        private async Task HandleMediaQueueAddAction(NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems == null)
            {
                return;
            }

            var newMediaFiles = new List<IMediaFile>();
            foreach (var newItem in e.NewItems)
            {
                if (newItem is IMediaFile mediaFile)
                {
                    newMediaFiles.Add(mediaFile);
                }
                if (newItem is IEnumerable<IMediaFile> mediaFiles)
                {
                    newMediaFiles.AddRange(mediaFiles);
                }

                foreach (var newMediaFile in newMediaFiles)
                {
                    PlaybackList.Items.Add(await CreateMediaPlaybackItem(newMediaFile));
                }
            }
        }
    }
}
