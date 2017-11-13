using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
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

            var playbackItem = new MediaPlaybackItem(mediaSource);
            UpdatePlaybackItemDisplayProperties(mediaFile, playbackItem);
            return playbackItem;
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

        protected MediaPlaybackItem RetrievePlaylistItem(IMediaFile mediaFile)
        {
            if (string.IsNullOrWhiteSpace(mediaFile?.Url))
            {
                return null;
            }

            return PlaybackList.Items.FirstOrDefault(i => i.Source?.Uri?.AbsoluteUri == mediaFile.Url);
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
                    // The reality is that this scenario is never going to happen. Even when we re-order or shuffle the list is being regenerated (Reset)
                    break;
                case NotifyCollectionChangedAction.Remove:
                    HandleMediaQueueRemoveAction(e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    await HandleMediaQueueReplaceAction(e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    await HandleMediaQueueResetAction(sender as IEnumerable<IMediaFile>);
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
                else if (newItem is IEnumerable<IMediaFile> mediaFiles)
                {
                    newMediaFiles.AddRange(mediaFiles);
                }

                foreach (var newMediaFile in newMediaFiles)
                {
                    PlaybackList.Items.Add(await CreateMediaPlaybackItem(newMediaFile));
                }
            }
        }

        private async Task HandleMediaQueueReplaceAction(NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems == null || e.OldItems == null)
            {
                return;
            }

            try
            {
                var newMediaFile = e.NewItems[0] as IMediaFile;
                var oldMediaFile = e.OldItems[0] as IMediaFile;

                if (newMediaFile == null || oldMediaFile == null)
                {
                    return;
                }

                var mediaFileInPlaylist = RetrievePlaylistItem(oldMediaFile);
                if (mediaFileInPlaylist == null)
                {
                    return;
                }

                if (newMediaFile == oldMediaFile || newMediaFile.Url == oldMediaFile.Url)
                {
                    // Update same media file
                    UpdatePlaybackItemDisplayProperties(newMediaFile, mediaFileInPlaylist);
                }
                else
                {
                    // Replace playlist media file with new one
                    var mediaFileInPlaylistIndex = PlaybackList.Items.IndexOf(mediaFileInPlaylist);
                    if (mediaFileInPlaylistIndex == PlaybackList.CurrentItemIndex)
                    {
                        Player.Pause();
                    }

                    PlaybackList.Items.RemoveAt(mediaFileInPlaylistIndex);

                    var newPlaybackItem = await CreateMediaPlaybackItem(newMediaFile);
                    PlaybackList.Items.Insert(mediaFileInPlaylistIndex, newPlaybackItem);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void HandleMediaQueueRemoveAction(NotifyCollectionChangedEventArgs e)
        {
            if (e?.OldItems == null)
            {
                return;
            }

            foreach (var oldItem in e.OldItems)
            {
                if (oldItem is IMediaFile mediaFile)
                {
                    var mediaFileInPlaylist = RetrievePlaylistItem(mediaFile);
                    if (mediaFileInPlaylist == null)
                    {
                        continue;
                    }

                    var mediaFileInPlaylistIndex = PlaybackList.Items.IndexOf(mediaFileInPlaylist);
                    var isMediaFileInPlaylistIndexCurrentlyPlaying = mediaFileInPlaylistIndex == PlaybackList.CurrentItemIndex;
                    if (isMediaFileInPlaylistIndexCurrentlyPlaying)
                    {
                        Player.Pause();
                    }

                    PlaybackList.Items.RemoveAt(mediaFileInPlaylistIndex);
                    if (PlaybackList.Items.Any() && isMediaFileInPlaylistIndexCurrentlyPlaying)
                    {
                        if (mediaFileInPlaylistIndex == 0)
                        {
                            PlaybackList.MoveNext();
                        }
                        else
                        {
                            PlaybackList.MovePrevious();
                        }
                    }
                }
            }
        }

        private async Task HandleMediaQueueResetAction(IEnumerable<IMediaFile> mediaFiles)
        {
            if (mediaFiles == null)
            {
                return;
            }

            if (Player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                Player.Pause();
            }

            PlaybackList.Items.Clear();
            foreach (var mediaFile in mediaFiles)
            {
                var newPlaybackItem = await CreateMediaPlaybackItem(mediaFile);
                PlaybackList.Items.Add(newPlaybackItem);
            }
        }

        private void UpdatePlaybackItemDisplayProperties(IMediaFile mediaFile, MediaPlaybackItem playbackItem)
        {
            if (mediaFile?.Metadata == null || playbackItem == null)
            {
                return;
            }

            var playbaItemDisplayProperties = playbackItem.GetDisplayProperties();
            playbaItemDisplayProperties.Type = mediaFile.Type == MediaFileType.Audio ? MediaPlaybackType.Music : MediaPlaybackType.Video;
            switch (playbaItemDisplayProperties.Type)
            {
                case MediaPlaybackType.Music:
                    if (!string.IsNullOrWhiteSpace(mediaFile.Metadata.Title))
                    {
                        playbaItemDisplayProperties.MusicProperties.Title = mediaFile.Metadata.Title;
                    }
                    break;
                case MediaPlaybackType.Video:
                    if (!string.IsNullOrWhiteSpace(mediaFile.Metadata.Title))
                    {
                        playbaItemDisplayProperties.VideoProperties.Title = mediaFile.Metadata.Title;
                    }
                    break;
            }
            if (!string.IsNullOrWhiteSpace(mediaFile.Metadata?.DisplayIconUri))
            {
                playbaItemDisplayProperties.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(mediaFile.Metadata.DisplayIconUri));
            }
            playbackItem.ApplyDisplayProperties(playbaItemDisplayProperties);
        }
    }
}
