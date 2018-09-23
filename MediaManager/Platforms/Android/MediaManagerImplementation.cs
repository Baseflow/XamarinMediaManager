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
                    _mediaExtractor = new MediaExtractor(Resources.System, RequestHeaders);
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

        public override async Task Pause()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().Pause();
        }

        public override async Task Play()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().Play();
        }

        public override async Task<IMediaItem> Play(string uri)
        {
            await MediaBrowserManager.EnsureInitialized();

            var mediaItem = await CrossMediaManager.Current.MediaExtractor.CreateMediaItem(uri);
            MediaQueue.Clear();
            MediaQueue.Add(mediaItem);

            var mediaUri = global::Android.Net.Uri.Parse(uri);
            MediaBrowserManager.MediaController.GetTransportControls().PlayFromUri(mediaUri, null);
            return mediaItem;
        }

        public override async Task Play(IMediaItem mediaItem)
        {
            await MediaBrowserManager.EnsureInitialized();

            MediaQueue.Clear();
            MediaQueue.Add(mediaItem);

            var mediaUri = global::Android.Net.Uri.Parse(mediaItem.MediaUri);
            MediaBrowserManager.MediaController.GetTransportControls().PlayFromUri(mediaUri, null);
        }

        public override async Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
        {
            await MediaBrowserManager.EnsureInitialized();

            MediaQueue.Clear();
            foreach (var url in items)
            {
                var mediaItem = new MediaItem(url);
                MediaQueue.Add(mediaItem);
            }

            await MediaQueue.FirstOrDefault()?.FetchMediaItemMetaData();
            MediaBrowserManager.MediaController.GetTransportControls().Prepare();

            //TODO: Need to do all of this in the background thread
            return await MediaQueue.FetchMediaQueueMetaData();
        }

        public override async Task Play(IEnumerable<IMediaItem> items)
        {
            await MediaBrowserManager.EnsureInitialized();

            MediaQueue.Clear();
            foreach (var item in items)
            {
                MediaQueue.Add(item);
            }

            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
        }

        public override Task<IMediaItem> Play(FileInfo file)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo)
        {
            throw new NotImplementedException();
        }

        public override async Task PlayNext()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().SkipToNext();
        }

        public override async Task PlayPrevious()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().SkipToPrevious();
        }

        public override async Task SeekTo(TimeSpan position)
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().SeekTo((long)position.TotalMilliseconds);
        }

        public override async Task SeekToStart()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().SeekTo(0);
        }

        public override async Task StepBackward()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().Rewind();
        }

        public override async Task StepForward()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().FastForward();
        }

        public override async Task Stop()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().Stop();
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
