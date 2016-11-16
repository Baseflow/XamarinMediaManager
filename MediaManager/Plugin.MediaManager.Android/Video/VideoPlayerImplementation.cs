using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content.Res;
using Android.Media;
using Android.Widget;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class VideoPlayerImplementation : Java.Lang.Object, 
        IVideoPlayer,
        MediaPlayer.IOnCompletionListener,
        MediaPlayer.IOnErrorListener,
        MediaPlayer.IOnPreparedListener
    {
        public IVideoSurface RenderSurface { get; private set; }
        public void SetVideoSurface(IVideoSurface videoSurface)
        {
            if (!(videoSurface is VideoSurface))
                throw new ArgumentException("Not a valid video surface");

            var canvas = (VideoSurface)videoSurface;
            RenderSurface = canvas;
        }

        public VideoAspectMode AspectMode { get; }

        public void SetAspectMode(VideoAspectMode aspectMode)
        {
            throw new NotImplementedException();
        }

        VideoView VideoViewCanvas => (VideoView)RenderSurface;

        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFailedEventHandler MediaFailed;
        public event MediaFileFailedEventHandler MediaFileFailed;
        public event MediaFileChangedEventHandler MediaFileChanged;
        public event MediaFinishedEventHandler MediaFinished;
        public event PlayingChangedEventHandler PlayingChanged;
        public event StatusChangedEventHandler StatusChanged;

        public TimeSpan Buffered => TimeSpan.FromSeconds(VideoViewCanvas.BufferPercentage);

        public TimeSpan Duration => TimeSpan.FromSeconds(VideoViewCanvas.Duration);

        public TimeSpan Position => TimeSpan.FromSeconds(VideoViewCanvas.CurrentPosition);

        public MediaPlayerStatus Status => MediaPlayerStatus.Playing;

        public async Task Play(IEnumerable<IMediaFile> mediaFiles)
        {
            await Play(mediaFiles?.ToList().FirstOrDefault());
        }

        public async Task Pause()
        {
            VideoViewCanvas.Pause();
        }

        public void Init()
        {
            var mediaController = new MediaController(Application.Context);
            mediaController.SetAnchorView(VideoViewCanvas);
            VideoViewCanvas.SetMediaController(mediaController);

            VideoViewCanvas.SetOnCompletionListener(this);
            VideoViewCanvas.SetOnErrorListener(this);
            VideoViewCanvas.SetOnPreparedListener(this);
        }

        public async Task Play(IMediaFile mediaFile)
        {
            if (VideoViewCanvas == null)
            {
                await Task.Delay(100);
                Init();
            }

            VideoViewCanvas.SetVideoURI(Android.Net.Uri.Parse(mediaFile.Url));
            VideoViewCanvas.Start();
        }

        public async Task Seek(TimeSpan position)
        {
            VideoViewCanvas.SeekTo(Convert.ToInt32(position.TotalMilliseconds));
        }

        public Dictionary<string, string> RequestHeaders { get; set; }

        public async Task Stop()
        {
            VideoViewCanvas.StopPlayback();
        }

        public void OnCompletion(MediaPlayer mp)
        {
            //OnMediaFinished(new MediaFinishedEventArgs(CurrentFile));
        }

        public bool OnError(MediaPlayer mp, MediaError what, int extra)
        {
            //SessionManager.UpdatePlaybackState(PlaybackStateCompat.StateError, Position.Seconds);
            Stop();
            return true;
        }

        public void OnPrepared(MediaPlayer mp)
        {
            //mp.Start();
            //SessionManager.UpdatePlaybackState(PlaybackStateCompat.StatePlaying, Position.Seconds);
        }
    }
}
