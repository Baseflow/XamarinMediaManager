using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content.Res;
using Android.Widget;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class VideoPlayerImplementation : IVideoPlayer
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

        VideoView VideoViewCanvas { 
            get 
            { 
                return (VideoView)RenderSurface;
            } 
        }

        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFailedEventHandler MediaFailed;
        public event MediaFileFailedEventHandler MediaFileFailed;
        public event MediaFileChangedEventHandler MediaFileChanged;
        public event MediaFinishedEventHandler MediaFinished;
        public event PlayingChangedEventHandler PlayingChanged;
        public event StatusChangedEventHandler StatusChanged;

        public TimeSpan Buffered
        {
            get
            {
                return TimeSpan.FromSeconds(VideoViewCanvas.BufferPercentage);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return TimeSpan.FromSeconds(VideoViewCanvas.Duration);
            }
        }

        public TimeSpan Position
        {
            get
            {
                return TimeSpan.FromSeconds(VideoViewCanvas.CurrentPosition);
            }
        }

        public MediaPlayerStatus Status
        {
            get
            {
                return MediaPlayerStatus.Playing;
            }
        }

        public async Task Play(IEnumerable<IMediaFile> mediaFiles)
        {
            await Play(mediaFiles?.ToList().FirstOrDefault());
        }

        public async Task Pause()
        {
            VideoViewCanvas.Pause();
        }

        public async Task Play(IMediaFile mediaFile)
        {
            VideoViewCanvas.SetVideoURI(Android.Net.Uri.Parse(mediaFile.Url));

            var mediaController = new MediaController(Application.Context);
            mediaController.SetAnchorView(VideoViewCanvas);
            VideoViewCanvas.SetMediaController(mediaController);
            VideoViewCanvas.Start();
        }

        public async Task Seek(TimeSpan position)
        {
            VideoViewCanvas.SeekTo(Convert.ToInt32(position.TotalMilliseconds));
        }

        public Dictionary<string, string> RequestProperties { get; set; }

        public async Task Stop()
        {
            VideoViewCanvas.StopPlayback();
        }
    }
}
