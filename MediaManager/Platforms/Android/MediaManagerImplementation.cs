using Android.App;
using Android.Content;
using MediaManager.Media;
using MediaManager.Platforms.Android;
using MediaManager.Playback;
using MediaManager.Video;
using MediaManager.Volume;
using MediaManager.Platforms.Android.Audio;
using System.Threading.Tasks;
using MediaManager.Platforms.Android.Utils;
using MediaManager.Audio;
using Android.Support.V4.Media.Session;
using Android.Content.Res;
using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace MediaManager
{
    public class MediaManagerImplementation : IMediaManager
    {
        public MediaManagerImplementation()
        {
        }

        public Context Context { get; set; } = Application.Context;
        public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();

        private MediaBrowserManager _mediaBrowserManager;
        public virtual MediaBrowserManager MediaBrowserManager
        {
            get
            {
                if (_mediaBrowserManager == null)
                    _mediaBrowserManager = new MediaBrowserManager(this);
                return _mediaBrowserManager;
            }
        }

        private IAudioPlayer _audioPlayer;
        public virtual IAudioPlayer AudioPlayer
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
        public virtual IVideoPlayer VideoPlayer
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
        public virtual IVolumeManager VolumeManager
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

        private IMediaQueue _mediaQueue;
        public virtual IMediaQueue MediaQueue
        {
            get
            {
                if (_mediaQueue == null)
                    _mediaQueue = new MediaQueue();

                return _mediaQueue;
            }
            set
            {
                _mediaQueue = value;
            }
        }

        private IMediaExtractor _mediaExtractor;
        public virtual IMediaExtractor MediaExtractor
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

        public event PropertyChangedEventHandler PropertyChanged;

        public TimeSpan Position => MediaBrowserManager?.PlaybackState?.Position ?? TimeSpan.Zero;

        public TimeSpan Duration => TimeSpan.FromMilliseconds(MediaBrowserManager?.Metadata?.Duration ?? 0);

        public TimeSpan Buffered => MediaBrowserManager?.PlaybackState?.Buffered ?? TimeSpan.Zero;

        public MediaPlayerStatus Status => MediaBrowserManager?.PlaybackState?.Status ?? MediaPlayerStatus.Stopped;

        public async Task Pause()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().Pause();
        }

        public async Task Play()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().Play();
        }

        public async Task Play(IMediaItem mediaItem)
        {
            await MediaBrowserManager.EnsureInitialized();
            var mediaUri = global::Android.Net.Uri.Parse(mediaItem.MetadataMediaUri);
            MediaBrowserManager.MediaController.GetTransportControls().PlayFromUri(mediaUri, null);
        }

        public async Task PlayNext()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().SkipToNext();
        }

        public async Task PlayPrevious()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().SkipToPrevious();
        }

        public async Task SeekTo(TimeSpan position)
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().SeekTo((long)position.TotalMilliseconds);
        }

        public async Task SeekToStart()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().SeekTo(0);
        }

        public async Task StepBackward()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().Rewind();
        }

        public async Task StepForward()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().FastForward();
        }

        public async Task Stop()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().Stop();
        }

        public void ToggleRepeat()
        {
            MediaBrowserManager.MediaController.GetTransportControls().SetRepeatMode(0);
        }

        public void ToggleShuffle()
        {
            MediaBrowserManager.MediaController.GetTransportControls().SetShuffleMode(0);
        }
    }
}
