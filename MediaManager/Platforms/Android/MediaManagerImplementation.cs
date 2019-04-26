using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Com.Google.Android.Exoplayer2;
using MediaManager.Media;
using MediaManager.Platforms.Android;
using MediaManager.Platforms.Android.Media;
using MediaManager.Platforms.Android.MediaSession;
using MediaManager.Platforms.Android.Playback;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Volume;
using NotificationManager = MediaManager.Platforms.Android.NotificationManager;

[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessWifiState)]
[assembly: UsesPermission(Android.Manifest.Permission.ForegroundService)]
namespace MediaManager
{
    [global::Android.Runtime.Preserve(AllMembers = true)]    
    public class MediaManagerImplementation : MediaManagerBase<MediaPlayer, SimpleExoPlayer>
    {
        public MediaManagerImplementation()
        {
        }

        public Context Context { get; set; } = Application.Context;

        public override void Init()
        {
            IsInitialized = MediaBrowserManager.Init();
        }

        private MediaBrowserManager _mediaBrowserManager;
        public virtual MediaBrowserManager MediaBrowserManager
        {
            get
            {
                if (_mediaBrowserManager == null)
                    _mediaBrowserManager = new MediaBrowserManager(Context);
                return _mediaBrowserManager;
            }
        }

        private IMediaPlayer _mediaPlayer;
        public override IMediaPlayer MediaPlayer
        {
            get
            {
                if (_mediaPlayer == null)
                    _mediaPlayer = new MediaPlayer();
                return _mediaPlayer;
            }
            set
            {
                _mediaPlayer = value;
            }
        }

        private INotificationManager _notificationManager;
        public virtual INotificationManager NotificationManager
        {
            get
            {
                if (_notificationManager == null)
                    _notificationManager = new NotificationManager();
                return _notificationManager;
            }
            set
            {
                _notificationManager = value;
            }
        }

        private IVolumeManager _volumeManager;
        public override IVolumeManager VolumeManager
        {
            get
            {
                if (_volumeManager == null)
                    _volumeManager = new VolumeManager(this);
                return _volumeManager;
            }
            set
            {
                _volumeManager = value;
            }
        }

        private IMediaExtractor _mediaExtractor;
        public override IMediaExtractor MediaExtractor
        {
            get
            {
                if (_mediaExtractor == null)
                    _mediaExtractor = new MediaExtractor();
                return _mediaExtractor;
            }
            set
            {
                _mediaExtractor = value;
            }
        }

        public override TimeSpan Position => TimeSpan.FromMilliseconds(MediaBrowserManager?.MediaController.PlaybackState?.Position ?? 0);

        public override TimeSpan Duration => MediaBrowserManager?.MediaController.Metadata?.ToMediaItem().Duration ?? TimeSpan.Zero;

        public override TimeSpan Buffered => TimeSpan.FromMilliseconds(MediaBrowserManager?.MediaController?.PlaybackState?.BufferedPosition ?? 0);

        public override MediaPlayerState State => MediaBrowserManager?.MediaController?.PlaybackState?.ToMediaPlayerState() ?? MediaPlayerState.Stopped;

        public override float Speed { get => MediaBrowserManager?.MediaController.PlaybackState?.PlaybackSpeed ?? 0; set => throw new NotImplementedException(); }


        public override Task Pause()
        {
            MediaBrowserManager.MediaController.GetTransportControls().Pause();
            return Task.CompletedTask;
        }

        public override Task Play()
        {
            MediaBrowserManager.MediaController.GetTransportControls().Play();
            return Task.CompletedTask;
        }

        public override async Task<IMediaItem> Play(string uri)
        {
            var mediaItem = await MediaExtractor.CreateMediaItem(uri);
            await AddMediaItemsToQueue(new List<IMediaItem> { mediaItem }, true);

            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return mediaItem;
        }

        public override async Task Play(IMediaItem mediaItem)
        {
            await AddMediaItemsToQueue(new List<IMediaItem> { mediaItem }, true);

            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return;
        }

        public override async Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
        {
            List<IMediaItem> mediaItems = new List<IMediaItem>();
            foreach (var uri in items)
            {
                mediaItems.Add(await MediaExtractor.CreateMediaItem(uri));
            }

            await AddMediaItemsToQueue(mediaItems, true);

            await MediaQueue.FirstOrDefault()?.FetchMetaData();
            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return MediaQueue;
        }

        public override async Task Play(IEnumerable<IMediaItem> items)
        {
            await AddMediaItemsToQueue(items, true);

            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return;
        }

        public override async Task<IMediaItem> Play(FileInfo file)
        {
            var mediaItem = await MediaExtractor.CreateMediaItem(file);
            var mediaItemToPlay = await AddMediaItemsToQueue(new List<IMediaItem> { mediaItem }, true);

            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return mediaItem;
        }

        public override async Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo)
        {
            List<IMediaItem> mediaItems = new List<IMediaItem>();
            foreach (var file in directoryInfo.GetFiles())
            {
                var mediaItem = await MediaExtractor.CreateMediaItem(file);
                mediaItems.Add(mediaItem);
            }

            await AddMediaItemsToQueue(mediaItems, true);

            await MediaQueue.FirstOrDefault()?.FetchMetaData();
            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return MediaQueue;
        }

        public override Task<bool> PlayNext()
        {
            if (NativeMediaPlayer.Player.NextWindowIndex == NativeMediaPlayer.Player.CurrentWindowIndex)
            {
                SeekTo(TimeSpan.FromSeconds(0));
                return Task.FromResult(true);
            }

            if (NativeMediaPlayer.Player.NextWindowIndex == -1)
            {
                return Task.FromResult(false);
            }

            MediaBrowserManager.MediaController.GetTransportControls().SkipToNext();

            return Task.FromResult(true);
        }

        public override Task<bool> PlayPrevious()
        {
            if (NativeMediaPlayer.Player.PreviousWindowIndex == NativeMediaPlayer.Player.CurrentWindowIndex)
            {
                SeekTo(TimeSpan.FromSeconds(0));
                return Task.FromResult(true);
            }

            if (NativeMediaPlayer.Player.PreviousWindowIndex == -1)
            {
                return Task.FromResult(false);
            }

            MediaBrowserManager.MediaController.GetTransportControls().SkipToPrevious();

            return Task.FromResult(true);
        }

        public override Task SeekTo(TimeSpan position)
        {
            MediaBrowserManager.MediaController.GetTransportControls().SeekTo((long)position.TotalMilliseconds);
            return Task.CompletedTask;
        }

        public override Task Stop()
        {
            MediaBrowserManager.MediaController.GetTransportControls().Stop();
            return Task.CompletedTask;
        }

        public override RepeatMode RepeatMode
        {
            get
            {
                return NativeMediaPlayer.RepeatMode;
            }
            set
            {
                MediaBrowserManager.MediaController.GetTransportControls().SetRepeatMode((int)value);
            }
        }

        public override ShuffleMode ShuffleMode
        {
            get
            {
                return (ShuffleMode)MediaBrowserManager.MediaController.ShuffleMode;
            }
            set
            {
                MediaBrowserManager.MediaController.GetTransportControls().SetShuffleMode((int)value);
            }
        }
    }
}
