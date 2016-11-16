using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content.Res;
using Android.Media;
using Android.Runtime;
using Android.Widget;
using Plugin.MediaManager.Abstractions;
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

        public VideoAspectMode AspectMode { get; set; }

        VideoView VideoViewCanvas => RenderSurface as VideoView;

        public IMediaFile CurrentFile { get; set; }
        private Android.Net.Uri currentUri { get; set; }

        public bool UseNativeControls = false;

        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFailedEventHandler MediaFailed;
        public event MediaFileFailedEventHandler MediaFileFailed;
        public event MediaFileChangedEventHandler MediaFileChanged;
        public event MediaFinishedEventHandler MediaFinished;
        public event PlayingChangedEventHandler PlayingChanged;
        public event StatusChangedEventHandler StatusChanged;

        public TimeSpan Buffered => VideoViewCanvas == null ? TimeSpan.Zero : TimeSpan.FromSeconds(VideoViewCanvas.BufferPercentage);

        public TimeSpan Duration => VideoViewCanvas == null ? TimeSpan.Zero : TimeSpan.FromSeconds(VideoViewCanvas.Duration);

        public TimeSpan Position => VideoViewCanvas == null ? TimeSpan.Zero : TimeSpan.FromSeconds(VideoViewCanvas.CurrentPosition);

        private MediaPlayerStatus _status;
        public MediaPlayerStatus Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                StatusChanged?.Invoke(this, new StatusChangedEventArgs(_status));
            }
        }

        public async Task Pause()
        {
            VideoViewCanvas.Pause();
            Status = MediaPlayerStatus.Paused;
            await Task.CompletedTask;
        }

        public void Init()
        {
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

            if (mediaFile != null)
            {
                CurrentFile = mediaFile;
                currentUri = Android.Net.Uri.Parse(mediaFile.Url);
            }

            if (Status == MediaPlayerStatus.Paused)
            {
                Status = MediaPlayerStatus.Playing;
                //We are simply paused so just continue
                VideoViewCanvas.Resume();
                return;
            }

            try
            {
                Status = MediaPlayerStatus.Buffering;

                VideoViewCanvas.SetVideoURI(currentUri);
                VideoViewCanvas.Start();
                Status = MediaPlayerStatus.Playing;
            }
            catch(Exception ex)
            {
                MediaFailed?.Invoke(this, new MediaFailedEventArgs(ex.Message, ex));
                Status = MediaPlayerStatus.Stopped;
            }
        }

        public async Task Seek(TimeSpan position)
        {
            VideoViewCanvas.SeekTo(Convert.ToInt32(position.TotalMilliseconds));
            await Task.CompletedTask;
        }

        public Dictionary<string, string> RequestHeaders { get; set; }

        public async Task Stop()
        {
            VideoViewCanvas.StopPlayback();
            Status = MediaPlayerStatus.Stopped;
            await Task.CompletedTask;
        }

        public void OnCompletion(MediaPlayer mp)
        {
            MediaFinished?.Invoke(this, new MediaFinishedEventArgs(CurrentFile));
        }

        public bool OnError(MediaPlayer mp, MediaError what, int extra)
        {
            Stop();
            Status = MediaPlayerStatus.Failed;
            return true;
        }

        public void OnPrepared(MediaPlayer mp)
        {
            Status = MediaPlayerStatus.Playing;
        }

        public bool OnInfo(MediaPlayer mp, [GeneratedEnum] MediaInfo what, int extra)
        {
            var test = mp.Duration;
            return true;
        }
    }
}
