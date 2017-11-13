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
using Plugin.MediaManager.Interfaces;

namespace Plugin.MediaManager
{
    public class VideoPlayerImplementation : BasePlayerImplementation, IVideoPlayer
    {
        private readonly IVolumeManager _volumeManager;
        private readonly Timer _playProgressTimer;
        private MediaSource _currentMediaSource;
        private TaskCompletionSource<bool> _loadMediaTaskCompletionSource = new TaskCompletionSource<bool>();
        private MediaPlayerStatus _status;
        private IMediaFile _currentMediaFile;
        private SpriteVisual _spriteVisual;
        private IVideoSurface _renderSurface;

        public VideoPlayerImplementation(IMediaPlyerPlaybackController mediaPlyerPlaybackController, IVolumeManager volumeManager)
            : base(mediaPlyerPlaybackController)
        {
            _volumeManager = volumeManager;
            _playProgressTimer = new Timer(state =>
            {
                if (Player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                {
                    var progress = Player.PlaybackSession.Position.TotalSeconds /
                                   Player.PlaybackSession.NaturalDuration.TotalSeconds;
                    if (double.IsInfinity(progress))
                        progress = 0;
                    PlayingChanged?.Invoke(this, new PlayingChangedEventArgs(progress, Player.PlaybackSession.Position, Player.PlaybackSession.NaturalDuration));
                }
            }, null, 0, int.MaxValue);

            Player.MediaFailed += (sender, args) =>
            {
                _status = MediaPlayerStatus.Failed;
                _playProgressTimer.Change(0, int.MaxValue);
                MediaFailed?.Invoke(this, new MediaFailedEventArgs(args.ErrorMessage, args.ExtendedErrorCode));
            };

            Player.PlaybackSession.PlaybackStateChanged += (sender, args) =>
            {
                switch (sender.PlaybackState)
                {
                    case MediaPlaybackState.None:
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Opening:
                        Status = MediaPlayerStatus.Loading;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Buffering:
                        Status = MediaPlayerStatus.Buffering;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Playing:
                        if ((sender.PlaybackRate <= 0) && (sender.Position == TimeSpan.Zero))
                        {
                            Status = MediaPlayerStatus.Stopped;
                        }
                        else
                        {
                            Status = MediaPlayerStatus.Playing;
                            _playProgressTimer.Change(0, 50);
                        }
                        break;
                    case MediaPlaybackState.Paused:
                        Status = MediaPlayerStatus.Paused;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            Player.MediaEnded += (sender, args) => { MediaFinished?.Invoke(this, new MediaFinishedEventArgs(_currentMediaFile)); };
            Player.PlaybackSession.BufferingProgressChanged += (sender, args) =>
            {
                var bufferedTime =
                    TimeSpan.FromSeconds(Player.PlaybackSession.BufferingProgress *
                                         Player.PlaybackSession.NaturalDuration.TotalSeconds);
                BufferingChanged?.Invoke(this,
                    new BufferingChangedEventArgs(Player.PlaybackSession.BufferingProgress, bufferedTime));
            };

            Player.PlaybackSession.SeekCompleted += (sender, args) => { };
            Player.MediaOpened += (sender, args) => { _loadMediaTaskCompletionSource.SetResult(true); };
            int.TryParse((Player.Volume * 100).ToString(), out var vol);
            _volumeManager.CurrentVolume = vol;
            _volumeManager.Muted = Player.IsMuted;
            _volumeManager.VolumeChanged += VolumeManagerOnVolumeChanged;
        }

        private void VolumeManagerOnVolumeChanged(object sender, VolumeChangedEventArgs volumeChangedEventArgs)
        {
            Player.Volume = (double)volumeChangedEventArgs.NewVolume;
            Player.IsMuted = volumeChangedEventArgs.Muted;
        }

        public Dictionary<string, string> RequestHeaders { get; set; }

        public MediaPlayerStatus Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                StatusChanged?.Invoke(this, new StatusChangedEventArgs(_status));
            }
        }

        public event StatusChangedEventHandler StatusChanged;
        public event PlayingChangedEventHandler PlayingChanged;
        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFinishedEventHandler MediaFinished;
        public event MediaFailedEventHandler MediaFailed;

        public TimeSpan Buffered
        {
            get
            {
                if (Player == null) return TimeSpan.Zero;
                return
                    TimeSpan.FromMilliseconds(Player.PlaybackSession.BufferingProgress *
                                              Player.PlaybackSession.NaturalDuration.TotalMilliseconds);
            }
        }

        public TimeSpan Duration => Player?.PlaybackSession.NaturalDuration ?? TimeSpan.Zero;
        public TimeSpan Position => Player?.PlaybackSession.Position ?? TimeSpan.Zero;

        public Task Pause()
        {
            if (Player.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
                Player.Play();
            else
                Player.Pause();
            return Task.CompletedTask;
        }

        public async Task PlayPause()
        {
            if ((_status == MediaPlayerStatus.Paused) || (_status == MediaPlayerStatus.Stopped))
                await Play();
            else
                await Pause();
        }

        public async Task Play(IMediaFile mediaFile)
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
                Player.Source = mediaPlaybackList;
                Player.Play();
            }
            catch (Exception)
            {
                Debug.WriteLine("Unable to open url: " + mediaFile.Url);
            }
        }

        public async Task Seek(TimeSpan position)
        {
            Player.PlaybackSession.Position = position;
            await Task.CompletedTask;
        }

        public Task Stop()
        {
            Player.PlaybackSession.PlaybackRate = 0;
            Player.PlaybackSession.Position = TimeSpan.Zero;
            Status = MediaPlayerStatus.Stopped;
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

        private void ResizeVideoSurface(object sender, SizeChangedEventArgs e)
        {
            var newSize = new Size(e.NewSize.Width, e.NewSize.Height);
            Player.SetSurfaceSize(newSize);
            _spriteVisual.Size = new Vector2((float)newSize.Width, (float)newSize.Height);
        }

        private void SetVideoSurface(VideoSurface canvas)
        {
            var size = new Size(canvas.ActualWidth, canvas.ActualHeight);
            Player.SetSurfaceSize(size);

            var compositor = ElementCompositionPreview.GetElementVisual(canvas).Compositor;
            var surface = Player.GetSurface(compositor);

            _spriteVisual = compositor.CreateSpriteVisual();
            _spriteVisual.Size =
                new Vector2((float)canvas.ActualWidth, (float)canvas.ActualHeight);

            CompositionBrush brush = compositor.CreateSurfaceBrush(surface.CompositionSurface);
            _spriteVisual.Brush = brush;

            var container = compositor.CreateContainerVisual();
            container.Children.InsertAtTop(_spriteVisual);

            ElementCompositionPreview.SetElementChildVisual(canvas, container);
        }

        public VideoAspectMode AspectMode { get; set; }

        public bool IsMuted
        {
            get;
            set;
        }

        public void SetVolume(float leftVolume, float rightVolume)
        {
            throw new NotImplementedException();
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
                    Status = MediaPlayerStatus.Loading;
                    break;
                case MediaSourceState.Opening:
                    Status = MediaPlayerStatus.Loading;
                    break;
                case MediaSourceState.Failed:
                    Status = MediaPlayerStatus.Failed;
                    break;
                case MediaSourceState.Closed:
                    Status = MediaPlayerStatus.Stopped;
                    break;
                case MediaSourceState.Opened:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Task Play()
        {
            Player.PlaybackSession.PlaybackRate = 1;
            Player.Play();
            return Task.CompletedTask;
        }
    }
}