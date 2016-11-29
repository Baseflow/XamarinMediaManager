using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content.Res;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Java.Lang;
using Java.Util.Concurrent;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class VideoPlayerImplementation : Java.Lang.Object, 
        IVideoPlayer,
        MediaPlayer.IOnCompletionListener,
        MediaPlayer.IOnErrorListener,
        MediaPlayer.IOnPreparedListener,
        MediaPlayer.IOnInfoListener
    {
        public VideoPlayerImplementation()
        {
            _status = MediaPlayerStatus.Stopped;
			StatusChanged += (sender, args) => OnPlayingHandler(args);
        }

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
			var progress = (Position.TotalSeconds / Duration.TotalSeconds) * 100;
			var position = Position;
			var duration = Duration;

			PlayingChanged?.Invoke(this, new PlayingChangedEventArgs(
				progress >= 0 ? progress : 0,
				position.TotalSeconds >= 0 ? position : TimeSpan.Zero,
				duration.TotalSeconds >= 0 ? duration : TimeSpan.Zero));
		}

        private IVideoSurface _renderSurface;
        public IVideoSurface RenderSurface { 
            get {
                return _renderSurface;
            } 
            set {
                if (!(value is VideoSurface))
                    throw new ArgumentException("Not a valid video surface");

                var canvas = (VideoSurface)value;
                _renderSurface = canvas;
            }
        }

		private VideoAspectMode _aspectMode;
		public VideoAspectMode AspectMode { 
			get
			{
				return _aspectMode;
			}
			set {
				//TODO: Wrap videoplayer to respect aspectmode
				_aspectMode = value;
			} 
		}

        VideoView VideoViewCanvas => RenderSurface as VideoView;

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

        public TimeSpan Buffered => VideoViewCanvas == null ? TimeSpan.Zero : TimeSpan.FromSeconds(VideoViewCanvas.BufferPercentage);

        public TimeSpan Duration => VideoViewCanvas == null ? TimeSpan.Zero : TimeSpan.FromSeconds(VideoViewCanvas.Duration);

        public TimeSpan Position => VideoViewCanvas == null ? TimeSpan.Zero : TimeSpan.FromSeconds(VideoViewCanvas.CurrentPosition);
		private int lastPosition = 0;

        private MediaPlayerStatus _status;
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
            if (VideoViewCanvas == null)
            {
                await Task.Delay(100);
                Init();
            }

            if (mediaFile != null && CurrentFile != mediaFile)
            {
                CurrentFile = mediaFile;
                currentUri = Android.Net.Uri.Parse(mediaFile.Url);
            }

            if (Status == MediaPlayerStatus.Paused)
            {
				//We are simply paused so just continue
				VideoViewCanvas.SeekTo(lastPosition);
				VideoViewCanvas.Start();
                return;
            }

            try
            {
                Status = MediaPlayerStatus.Buffering;
				VideoViewCanvas.SetVideoURI(currentUri, RequestHeaders);
            }
            catch(System.Exception ex)
            {
				OnMediaFailed(new MediaFailedEventArgs(ex.Message, ex));
                Status = MediaPlayerStatus.Stopped;
            }
        }

        public async Task Seek(TimeSpan position)
        {
            VideoViewCanvas.SeekTo(Convert.ToInt32(position.TotalMilliseconds));
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
            OnMediaFinished(new MediaFinishedEventArgs(CurrentFile));
        }

        public bool OnError(MediaPlayer mp, MediaError what, int extra)
        {
			Stop().Wait();
            Status = MediaPlayerStatus.Failed;
			OnMediaFailed(new MediaFailedEventArgs(what.ToString(), new System.Exception()));
            return true;
        }

        public void OnPrepared(MediaPlayer mp)
        {
			if(Status == MediaPlayerStatus.Buffering)
				VideoViewCanvas.Start();

            Status = MediaPlayerStatus.Playing;
        }

        public bool OnInfo(MediaPlayer mp, [GeneratedEnum] MediaInfo what, int extra)
        {
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
