using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.App;
using Android.Content;
using Java.Lang;
using Java.Util.Concurrent;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager
{
    public class VideoPlayerImplementation : Java.Lang.Object,
        IVideoPlayer,
        MediaPlayer.IOnCompletionListener,
        MediaPlayer.IOnErrorListener,
        MediaPlayer.IOnPreparedListener,
        MediaPlayer.IOnInfoListener
    {
        private MediaPlayer _mediaPlayer;
        private AudioManager _audioManager = null;
        public VideoPlayerImplementation()
        {
            _audioManager = (AudioManager)Application.Context.GetSystemService(Context.AudioService);
            _status = MediaPlayerStatus.Stopped;
            StatusChanged += (sender, args) => OnPlayingHandler(args);
        }

        private bool isPlayerReady = false;

        private IScheduledExecutorService _executorService = Executors.NewSingleThreadScheduledExecutor();
        private IScheduledFuture _scheduledFuture;

        private void OnPlayingHandler(StatusChangedEventArgs args)
        {
            if (args.Status == MediaPlayerStatus.Playing)
            {
                CancelPlayingHandler();
                StartPlayingHandler();
            }
            if (args.Status == MediaPlayerStatus.Stopped || args.Status == MediaPlayerStatus.Failed || args.Status == MediaPlayerStatus.Paused)
                CancelPlayingHandler();
        }

        private void CancelPlayingHandler()
        {
            _scheduledFuture?.Cancel(false);
        }

        private void StartPlayingHandler()
        {
            var handler = new Handler();
            var runnable = new Runnable(() => { handler.Post(OnPlaying); });
            if (!_executorService.IsShutdown)
            {
                _scheduledFuture = _executorService.ScheduleAtFixedRate(runnable, 100, 1000, TimeUnit.Milliseconds);
            }
        }

        private void OnPlaying()
        {
            if (!IsReadyRendering)
                CancelPlayingHandler(); //RenderSurface is no longer valid => Cancel the periodic firing

            var progress = (Position.TotalSeconds / Duration.TotalSeconds);
            var position = Position;
            var duration = Duration;

            PlayingChanged?.Invoke(this, new PlayingChangedEventArgs(
                !double.IsInfinity(progress) ? progress : 0,
                position.TotalSeconds >= 0 ? position : TimeSpan.Zero,
                duration.TotalSeconds >= 0 ? duration : TimeSpan.Zero));
        }

        /// <summary>
        /// True when RenderSurface has been initialized and ready for rendering
        /// </summary>
        public bool IsReadyRendering => RenderSurface != null && !RenderSurface.IsDisposed;

        VideoView VideoViewCanvas => RenderSurface as VideoView;

        private IVideoSurface _renderSurface;
        public IVideoSurface RenderSurface
        {
            get
            {
                return _renderSurface;
            }
            set
            {
                if (!(value is VideoSurface))
                    throw new ArgumentException("Not a valid video surface");

                if (_renderSurface == value)
                    return;

                var canvas = (VideoSurface)value;
                _renderSurface = canvas;

                //New canvas object => need initialization
                isPlayerReady = false;
            }
        }

        private VideoAspectMode _aspectMode;
        public VideoAspectMode AspectMode
        {
            get
            {
                return _aspectMode;
            }
            set
            {
                //TODO: Wrap videoplayer to respect aspectmode
                _aspectMode = value;
            }
        }


        private bool _IsMuted = false;
        public bool IsMuted
        {
            get { return _IsMuted; }
            set
            {
                if (_IsMuted == value)
                    return;

                float volumeValue = 0.0f;
                if (!value)
                {
                    //https://developer.xamarin.com/api/member/Android.Media.AudioManager.GetStreamVolume/p/Android.Media.Stream/
                    //https://stackoverflow.com/questions/17898382/audiomanager-getstreamvolumeaudiomanager-stream-music-returns-0
                    Stream streamType = Stream.Music;
                    int volumeMax = _audioManager.GetStreamMaxVolume(streamType);
                    int volume = _audioManager.GetStreamVolume(streamType);

                    //ltang: Unmute with the current volume
                    volumeValue = (float)volume / volumeMax;
                }

                SetVolume(volumeValue, volumeValue);
                _IsMuted = value;
            }
        }

        public void SetVolume(float leftVolume, float rightVolume)
        {
            try
            {
                _mediaPlayer?.SetVolume(leftVolume, rightVolume);
            }
            catch (Java.Lang.IllegalStateException e)
            {
                //ltang: Wrong state to set volume
                throw;
            }
            catch (System.Exception e)
            {
                throw;
            }
        }

        public IMediaFile CurrentFile { get; set; }
        private Android.Net.Uri currentUri { get; set; }

        public Dictionary<string, string> RequestHeaders { get; set; }

        public bool UseNativeControls = false;

        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFailedEventHandler MediaFailed;
        public event MediaFileFailedEventHandler MediaFileFailed;
        public event MediaFileChangedEventHandler MediaFileChanged;
        public event MediaFinishedEventHandler MediaFinished;
        public event PlayingChangedEventHandler PlayingChanged;
        public event StatusChangedEventHandler StatusChanged;

        protected virtual void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }

        protected virtual void OnPlayingChanged(PlayingChangedEventArgs e)
        {
            PlayingChanged?.Invoke(this, e);
        }

        protected virtual void OnBufferingChanged(BufferingChangedEventArgs e)
        {
            BufferingChanged?.Invoke(this, e);
        }

        protected virtual void OnMediaFinished(MediaFinishedEventArgs e)
        {
            MediaFinished?.Invoke(this, e);
        }

        protected virtual void OnMediaFailed(MediaFailedEventArgs e)
        {
            MediaFailed?.Invoke(this, e);
        }

        protected virtual void OnMediaFileChanged(MediaFileChangedEventArgs e)
        {
            MediaFileChanged?.Invoke(this, e);
        }

        protected virtual void OnMediaFileFailed(MediaFileFailedEventArgs e)
        {
            MediaFileFailed?.Invoke(this, e);
        }

        public TimeSpan Buffered => IsReadyRendering == false ? TimeSpan.Zero : TimeSpan.FromSeconds(VideoViewCanvas.BufferPercentage);

        public TimeSpan Duration => IsReadyRendering == false ? TimeSpan.Zero : TimeSpan.FromMilliseconds(VideoViewCanvas.Duration);

        public TimeSpan Position => IsReadyRendering == false ? TimeSpan.Zero : TimeSpan.FromMilliseconds(VideoViewCanvas.CurrentPosition);

        private int lastPosition = 0;

        private MediaPlayerStatus _status = MediaPlayerStatus.Stopped;
        public MediaPlayerStatus Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                OnStatusChanged(new StatusChangedEventArgs(_status));
            }
        }

        public async Task Pause()
        {
            lastPosition = VideoViewCanvas.CurrentPosition;
            VideoViewCanvas.Pause();
            Status = MediaPlayerStatus.Paused;
            await Task.CompletedTask;
        }

        public void Init()
        {
            _mediaPlayer = null;

            Status = MediaPlayerStatus.Loading;

            if (UseNativeControls)
            {
                var mediaController = new MediaController(((VideoView)RenderSurface).Context);
                mediaController.SetAnchorView(VideoViewCanvas);
                VideoViewCanvas.SetMediaController(mediaController);
            }

            VideoViewCanvas.SetOnCompletionListener(this);
            VideoViewCanvas.SetOnErrorListener(this);
            VideoViewCanvas.SetOnPreparedListener(this);
            VideoViewCanvas.SetOnInfoListener(this);
        }

        public async Task Play(IMediaFile mediaFile = null)
        {
            if (!IsReadyRendering)
                //Android ViewRenderer might not initialize Control yet
                return;

            if (isPlayerReady == false)
            {
                //await Task.Delay(100);
                Init();
                isPlayerReady = true;
            }

            if (mediaFile == null || (mediaFile != null && string.IsNullOrEmpty(mediaFile.Url)))
            {
                return;
            }

            if (mediaFile != null && CurrentFile != mediaFile)
            {
                CurrentFile = mediaFile;
                currentUri = Android.Net.Uri.Parse(mediaFile.Url);
                VideoViewCanvas.StopPlayback();
                //VideoViewCanvas.Suspend();
                Status = MediaPlayerStatus.Stopped;
            }

            if (Status == MediaPlayerStatus.Paused)
            {
                //We are simply paused so just continue
                VideoViewCanvas.SeekTo(lastPosition);
                VideoViewCanvas.Start();

                Status = MediaPlayerStatus.Playing;
                return;
            }

            try
            {
                Status = MediaPlayerStatus.Buffering;
                VideoViewCanvas.SetVideoURI(currentUri, RequestHeaders);
            }
            catch (System.Exception ex)
            {
                OnMediaFailed(new MediaFailedEventArgs(ex.Message, ex));
                Status = MediaPlayerStatus.Stopped;
            }
        }

        public async Task Seek(TimeSpan position)
        {
            int msec = Convert.ToInt32(position.TotalMilliseconds);
            VideoViewCanvas.SeekTo(msec);
            lastPosition = VideoViewCanvas.CurrentPosition;

            await Task.CompletedTask;
        }

        public async Task Stop()
        {
            VideoViewCanvas.StopPlayback();
            Status = MediaPlayerStatus.Stopped;
            await Task.CompletedTask;
        }

        public void OnCompletion(MediaPlayer mp)
        {
            Console.WriteLine($"OnCompletion");

            OnMediaFinished(new MediaFinishedEventArgs(CurrentFile));
        }

        public bool OnError(MediaPlayer mp, MediaError what, int extra)
        {
            Console.WriteLine($"OnError: {what}");

            Stop().Wait();
            Status = MediaPlayerStatus.Failed;
            OnMediaFailed(new MediaFailedEventArgs(what.ToString(), new System.Exception()));
            return true;
        }

        public void OnPrepared(MediaPlayer mp)
        {
            Console.WriteLine($"OnPrepared: {Status}");

            _mediaPlayer = mp;

            if (Status == MediaPlayerStatus.Buffering)
                VideoViewCanvas.Start();

            Status = MediaPlayerStatus.Playing;
        }

        public bool OnInfo(MediaPlayer mp, [GeneratedEnum] MediaInfo what, int extra)
        {
            Console.WriteLine($"OnInfo: {what}");

            switch (what)
            {
                case MediaInfo.BadInterleaving:
                    break;
                case MediaInfo.BufferingEnd:
                    break;
                case MediaInfo.BufferingStart:
                    break;
                case MediaInfo.MetadataUpdate:
                    break;
                case MediaInfo.NotSeekable:
                    break;
                case MediaInfo.SubtitleTimedOut:
                    break;
                case MediaInfo.Unknown:
                    break;
                case MediaInfo.UnsupportedSubtitle:
                    break;
                case MediaInfo.VideoRenderingStart:
                    break;
                case MediaInfo.VideoTrackLagging:
                    break;
            }

            return true;
        }
    }
}
