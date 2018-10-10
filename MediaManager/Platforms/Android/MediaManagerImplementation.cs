using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Platforms.Android;
using MediaManager.Platforms.Android.Audio;
using MediaManager.Platforms.Android.Media;
using MediaManager.Platforms.Android.MediaSession;
using MediaManager.Platforms.Android.Playback;
using MediaManager.Platforms.Android.Video;
using MediaManager.Playback;
using MediaManager.Video;
using MediaManager.Volume;
using NotificationManager = MediaManager.Platforms.Android.NotificationManager;

namespace MediaManager
{
    [global::Android.Runtime.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : MediaManagerBase
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

        private IAudioPlayer _audioPlayer;
        public override IAudioPlayer AudioPlayer
        {
            get
            {
                if (_audioPlayer == null)
                    _audioPlayer = new AudioPlayer();
                return _audioPlayer;
            }
            set
            {
                _audioPlayer = value;
            }
        }

        private IVideoPlayer _videoPlayer;
        public override IVideoPlayer VideoPlayer
        {
            get
            {
                if (_videoPlayer == null)
                    _videoPlayer = new VideoPlayer();
                return _videoPlayer;
            }
            set
            {
                _videoPlayer = value;
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
            MediaQueue.Clear();
            MediaQueue.Add(mediaItem);

            var mediaUri = global::Android.Net.Uri.Parse(uri);
            MediaBrowserManager.MediaController.GetTransportControls().PlayFromUri(mediaUri, null);
            return mediaItem;
        }

        public override Task Play(IMediaItem mediaItem)
        {
            MediaQueue.Clear();
            MediaQueue.Add(mediaItem);

            var mediaUri = global::Android.Net.Uri.Parse(mediaItem.MediaUri);
            MediaBrowserManager.MediaController.GetTransportControls().PlayFromUri(mediaUri, null);
            return Task.CompletedTask;
        }

        public override async Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
        {
            MediaQueue.Clear();
            foreach (var url in items)
            {
                var mediaItem = await MediaExtractor.CreateMediaItem(url);
                MediaQueue.Add(mediaItem);
            }

            await MediaQueue.FirstOrDefault()?.FetchMetaData();
            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return MediaQueue;
        }

        public override Task Play(IEnumerable<IMediaItem> items)
        {
            MediaQueue.Clear();
            foreach (var item in items)
            {
                MediaQueue.Add(item);
            }

            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return Task.CompletedTask;
        }

        public override async Task<IMediaItem> Play(FileInfo file)
        {
            var mediaItem = await MediaExtractor.CreateMediaItem(file);
            var mediaUri = global::Android.Net.Uri.Parse(mediaItem.MediaUri);
            MediaBrowserManager.MediaController.GetTransportControls().PlayFromUri(mediaUri, null);
            return mediaItem;
        }

        public override async Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo)
        {
            MediaQueue.Clear();
            foreach (var file in directoryInfo.GetFiles())
            {
                var mediaItem = await MediaExtractor.CreateMediaItem(file);
                MediaQueue.Add(mediaItem);
            }

            await MediaQueue.FirstOrDefault()?.FetchMetaData();
            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return MediaQueue;
        }

        public override Task PlayNext()
        {
            MediaBrowserManager.MediaController.GetTransportControls().SkipToNext();
            return Task.CompletedTask;
        }

        public override Task PlayPrevious()
        {
            MediaBrowserManager.MediaController.GetTransportControls().SkipToPrevious();
            return Task.CompletedTask;
        }

        public override Task SeekTo(TimeSpan position)
        {
            MediaBrowserManager.MediaController.GetTransportControls().SeekTo((long)position.TotalMilliseconds);
            return Task.CompletedTask;
        }

        //TODO: Move to extension method
        public override Task SeekToStart()
        {
            MediaBrowserManager.MediaController.GetTransportControls().SeekTo(0);
            return Task.CompletedTask;
        }

        public override Task StepBackward()
        {
            MediaBrowserManager.MediaController.GetTransportControls().Rewind();
            return Task.CompletedTask;
        }

        public override Task StepForward()
        {
            MediaBrowserManager.MediaController.GetTransportControls().FastForward();
            return Task.CompletedTask;
        }

        public override Task Stop()
        {
            MediaBrowserManager.MediaController.GetTransportControls().Stop();
            return Task.CompletedTask;
        }

        public override void ToggleRepeat()
        {
            MediaBrowserManager.MediaController.GetTransportControls().SetRepeatMode(0);
        }

        public override void ToggleShuffle()
        {
            MediaBrowserManager.MediaController.GetTransportControls().SetShuffleMode(0);
        }
    }
}
