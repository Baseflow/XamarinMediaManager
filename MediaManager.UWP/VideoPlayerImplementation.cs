using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class VideoPlayerImplementation : IVideoPlayer
    {
        private readonly IVolumeManager _volumeManager;
        private readonly MediaPlayer _player;
        private readonly Timer _playProgressTimer;
        private MediaSource _currentMediaSource;
        private TaskCompletionSource<bool> _loadMediaTaskCompletionSource = new TaskCompletionSource<bool>();
        private PlaybackState _state;
        private IMediaItem _currentMediaFile;
        private SpriteVisual _spriteVisual;
        private IVideoSurface _renderSurface;

        public VideoPlayerImplementation(IVolumeManager volumeManager)
        {
            _volumeManager = volumeManager;
            _player = new MediaPlayer();

            _playProgressTimer = new Timer(state =>
            {
                if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                {
                    var progress = _player.PlaybackSession.Position.TotalSeconds/
                                   _player.PlaybackSession.NaturalDuration.TotalSeconds;
                    if (double.IsInfinity(progress))
                        progress = 0;
                    Playing?.Invoke(this, new PlayingChangedEventArgs(progress, _player.PlaybackSession.Position, _player.PlaybackSession.NaturalDuration));
                }
            }, null, 0, int.MaxValue);

            _player.MediaFailed += (sender, args) =>
            {
                _state = PlaybackState.Failed; 
                _playProgressTimer.Change(0, int.MaxValue);
                Media?.Invoke(this, new MediaFailedEventArgs(args.ErrorMessage, args.ExtendedErrorCode));
            };

            _player.PlaybackSession.PlaybackStateChanged += (sender, args) =>
            {
                switch (sender.PlaybackState)
                {
                    case MediaPlaybackState.None:
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Opening:
                        State = PlaybackState.Loading;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Buffering:
                        State = PlaybackState.Buffering;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Playing:
                        if ((sender.PlaybackRate <= 0) && (sender.Position == TimeSpan.Zero))
                        {
                            State = PlaybackState.Stopped;
                        }
                        else
                        {
                            State = PlaybackState.Playing;
                            _playProgressTimer.Change(0, 50);
                        }
                        break;
                    case MediaPlaybackState.Paused:
                        State = PlaybackState.Paused;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            _player.MediaEnded += (sender, args) => { Finished?.Invoke(this, new MediaFinishedEventArgs(_currentMediaFile)); };
            _player.PlaybackSession.BufferingProgressChanged += (sender, args) =>
            {
                var bufferedTime =
                    TimeSpan.FromSeconds(_player.PlaybackSession.BufferingProgress*
                                         _player.PlaybackSession.NaturalDuration.TotalSeconds);
                Buffering?.Invoke(this,
                    new BufferingChangedEventArgs(_player.PlaybackSession.BufferingProgress, bufferedTime));
            };

            _player.PlaybackSession.SeekCompleted += (sender, args) => { };
            _player.MediaOpened += (sender, args) => { _loadMediaTaskCompletionSource.SetResult(true); };
            _volumeManager.CurrentVolume = (float)_player.Volume;
            _volumeManager.Mute = _player.IsMuted;
            _volumeManager.VolumeChanged += VolumeManagerOnVolumeChanged;
        }

        private void VolumeManagerOnVolumeChanged(object sender, VolumeChangedEventArgs volumeChangedEventArgs)
        {
            _player.Volume = (double)volumeChangedEventArgs.Volume;
            _player.IsMuted = volumeChangedEventArgs.Mute;
        }

        public Dictionary<string, string> RequestHeaders { get; set; }

        public PlaybackState State
        {
            get { return _state; }
            private set
            {
                _state = value;
                Status?.Invoke(this, new StatusChangedEventArgs(_state));
            }
        }

        public event StatusChangedEventHandler Status;
        public event PlayingChangedEventHandler Playing;
        public event BufferingChangedEventHandler Buffering;
        public event MediaFinishedEventHandler Finished;
        public event MediaFailedEventHandler Failed;

        public TimeSpan Buffered
        {
            get
            {
                if (_player == null) return TimeSpan.Zero;
                return
                    TimeSpan.FromMilliseconds(_player.PlaybackSession.BufferingProgress*
                                              _player.PlaybackSession.NaturalDuration.TotalMilliseconds);
            }
        }

        public TimeSpan Duration => _player?.PlaybackSession.NaturalDuration ?? TimeSpan.Zero;
        public TimeSpan Position => _player?.PlaybackSession.Position ?? TimeSpan.Zero;

        public Task Pause()
        {
            if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
                _player.Play();
            else
                _player.Pause();
            return Task.CompletedTask;
        }

        public async Task PlayPause()
        {
            if ((_state == PlaybackState.Paused) || (_state == PlaybackState.Stopped))
                await Play();
            else
                await Pause();
        }

        public async Task Play(IMediaItem mediaFile)
        {
            _loadMediaTaskCompletionSource = new TaskCompletionSource<bool>();
            try
            {
                if (_currentMediaSource != null)
                {
                    _currentMediaSource.StateChanged -= MediaSourceOnStateChanged;
                    _currentMediaSource.OpenOperationCompleted -= MediaSourceOnOpenOperationCompleted;
                }
                // Todo: sync this with the playback queue
                var mediaPlaybackList = new MediaPlaybackList();
                _currentMediaSource = await CreateMediaSource(mediaFile);
                _currentMediaSource.StateChanged += MediaSourceOnStateChanged;
                _currentMediaSource.OpenOperationCompleted += MediaSourceOnOpenOperationCompleted;
                var item = new MediaPlaybackItem(_currentMediaSource);
                mediaPlaybackList.Items.Add(item);
                _player.Source = mediaPlaybackList;
                _player.Play();
            }
            catch (Exception)
            {
                Debug.WriteLine("Unable to open url: " + mediaFile.Url);
            }
        }

        public async Task Seek(TimeSpan position)
        {
            _player.PlaybackSession.Position = position;
            await Task.CompletedTask;
        }

        public Task Stop()
        {
            _player.PlaybackSession.PlaybackRate = 0;
            _player.PlaybackSession.Position = TimeSpan.Zero;
            State = PlaybackState.Stopped;
            return Task.CompletedTask;
        }

        /// <summary>
        /// True when RenderSurface has been initialized and ready for rendering
        /// </summary>
        public bool IsReadyRendering => RenderSurface != null && !RenderSurface.IsDisposed;        

        public IVideoSurface RenderSurface
        {
            get { return _renderSurface; }
            set
            {
                if (!(value is VideoSurface))
                    throw new ArgumentException("Not a valid video surface");

                _renderSurface = (VideoSurface)value;

                SetVideoSurface((VideoSurface)_renderSurface);

                if (_renderSurface != value)
                {
                    var canvas = _renderSurface as Canvas;
                    if (canvas != null)
                    {
                        canvas.SizeChanged -= ResizeVideoSurface;
                    }
                    _renderSurface = value;
                    canvas = _renderSurface as Canvas;
                    if (canvas != null)
                    {
                        canvas.SizeChanged += ResizeVideoSurface;
                    }
                }
            }
        }

        private void ResizeVideoSurface(object sender, SizeChangedEventArgs e)
        {
            var newSize = new Size(e.NewSize.Width, e.NewSize.Height);
            _player.SetSurfaceSize(newSize);
            _spriteVisual.Size = new Vector2((float)newSize.Width, (float)newSize.Height);
        }

        private void SetVideoSurface(VideoSurface canvas)
        {
            var size = new Size(canvas.ActualWidth, canvas.ActualHeight);
            _player.SetSurfaceSize(size);

            var compositor = ElementCompositionPreview.GetElementVisual(canvas).Compositor;
            var surface = _player.GetSurface(compositor);

            _spriteVisual = compositor.CreateSpriteVisual();
            _spriteVisual.Size =
                new Vector2((float) canvas.ActualWidth, (float) canvas.ActualHeight);

            CompositionBrush brush = compositor.CreateSurfaceBrush(surface.CompositionSurface);
            _spriteVisual.Brush = brush;

            var container = compositor.CreateContainerVisual();
            container.Children.InsertAtTop(_spriteVisual);

            ElementCompositionPreview.SetElementChildVisual(canvas, container);
        }

        public VideoAspectMode AspectMode { get; set; }

        private async Task<MediaSource> CreateMediaSource(IMediaItem mediaFile)
        {
            //switch (mediaFile.Availability)
            //{
            //    case ResourceAvailability.Remote:
            //        return MediaSource.CreateFromUri(new Uri(mediaFile.Url));
            //    case ResourceAvailability.Local:
            //        var du = _player.SystemMediaTransportControls.DisplayUpdater;
            //        var storageFile = await StorageFile.GetFileFromPathAsync(mediaFile.Url);
            //        var playbackType = mediaFile.Type == MediaItemType.Audio
            //            ? MediaPlaybackType.Music
            //            : MediaPlaybackType.Video;
            //        await du.CopyFromFileAsync(playbackType, storageFile);
            //        du.Update();
            //        return MediaSource.CreateFromStorageFile(storageFile);
            //}
            return MediaSource.CreateFromUri(new Uri(mediaFile.Url));
        }

        private void MediaSourceOnOpenOperationCompleted(MediaSource sender,
            MediaSourceOpenOperationCompletedEventArgs args)
        {
            if (args.Error != null)
                MediaFailed?.Invoke(this, new MediaFailedEventArgs(args.Error.ToString(), args.Error.ExtendedError));
        }

        private void MediaSourceOnStateChanged(MediaSource sender, MediaSourceStateChangedEventArgs args)
        {
            switch (args.NewState)
            {
                case MediaSourceState.Initial:
                    State = PlaybackState.Loading;
                    break;
                case MediaSourceState.Opening:
                    State = PlaybackState.Loading;
                    break;
                case MediaSourceState.Failed:
                    State = PlaybackState.Failed;
                    break;
                case MediaSourceState.Closed:
                    State = PlaybackState.Stopped;
                    break;
                case MediaSourceState.Opened:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Task Play()
        {
            _player.PlaybackSession.PlaybackRate = 1;
            _player.Play();
            return Task.CompletedTask;
        }
    }
}