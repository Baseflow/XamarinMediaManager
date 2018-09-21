using Android.App;
using Android.Content;
using MediaManager.Media;
using MediaManager.Platforms.Android;
using MediaManager.Playback;
using MediaManager.Video;
using MediaManager.Volume;
using MediaManager.Platforms.Android.Audio;
using System.Threading.Tasks;
using MediaManager.Audio;
using Android.Content.Res;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MediaManager.Platforms.Android.Media;
using MediaManager.Queue;
using NotificationManager = MediaManager.Platforms.Android.NotificationManager;
using MediaManager.Platforms.Android.MediaSession;
using MediaManager.Platforms.Android.Video;
using MediaManager.Platforms.Android.Playback;
using System.IO;

namespace MediaManager
{
    [global::Android.Runtime.Preserve(AllMembers = true)]
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
                    _mediaBrowserManager = new MediaBrowserManager(Context);
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
        public event PlayingChangedEventHandler PlayingChanged;
        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaItemFinishedEventHandler MediaItemFinished;
        public event MediaItemChangedEventHandler MediaItemChanged;
        public event MediaItemFailedEventHandler MediaItemFailed;
        public event StateChangedEventHandler StateChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TimeSpan Position => TimeSpan.FromMilliseconds(MediaBrowserManager?.MediaController.PlaybackState?.Position ?? 0);

        public TimeSpan Duration => MediaBrowserManager?.MediaController.Metadata?.ToMediaItem().Duration ?? TimeSpan.Zero;

        public TimeSpan Buffered => TimeSpan.FromMilliseconds(MediaBrowserManager?.MediaController?.PlaybackState?.BufferedPosition ?? 0);

        public MediaPlayerState State => MediaBrowserManager?.MediaController?.PlaybackState?.ToMediaPlayerState() ?? MediaPlayerState.Stopped;

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

        public async Task<IMediaItem> Play(string uri)
        {
            await MediaBrowserManager.EnsureInitialized();

            var mediaItem = await CrossMediaManager.Current.MediaExtractor.CreateMediaItem(uri);
            MediaQueue.Clear();
            MediaQueue.Add(mediaItem);

            var mediaUri = global::Android.Net.Uri.Parse(uri);
            MediaBrowserManager.MediaController.GetTransportControls().PlayFromUri(mediaUri, null);
            return mediaItem;
        }

        public async Task Play(IMediaItem mediaItem)
        {
            await MediaBrowserManager.EnsureInitialized();

            MediaQueue.Clear();
            MediaQueue.Add(mediaItem);

            var mediaUri = global::Android.Net.Uri.Parse(mediaItem.MediaUri);
            MediaBrowserManager.MediaController.GetTransportControls().PlayFromUri(mediaUri, null);
        }

        public async Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
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

        public async Task Play(IEnumerable<IMediaItem> items)
        {
            await MediaBrowserManager.EnsureInitialized();

            MediaQueue.Clear();
            foreach (var item in items)
            {
                MediaQueue.Add(item);
            }

            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
        }

        public Task<IMediaItem> Play(FileInfo file)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo)
        {
            throw new NotImplementedException();
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

        public void OnStateChanged(object sender, StateChangedEventArgs e) => StateChanged?.Invoke(sender, e);
        public void OnPlayingChanged(object sender, PlayingChangedEventArgs e) => PlayingChanged?.Invoke(sender, e);
        public void OnBufferingChanged(object sender, BufferingChangedEventArgs e) => BufferingChanged?.Invoke(sender, e);
        public void OnMediaItemFinished(object sender, MediaItemEventArgs e) => MediaItemFinished?.Invoke(sender, e);
        public void OnMediaItemChanged(object sender, MediaItemEventArgs e) => MediaItemChanged?.Invoke(sender, e);
        public void OnMediaItemFailed(object sender, MediaItemFailedEventArgs e) => MediaItemFailed?.Invoke(sender, e);
    }
}
