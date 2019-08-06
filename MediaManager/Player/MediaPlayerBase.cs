using System;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Video;

namespace MediaManager.Player
{
    public abstract class MediaPlayerBase : NotifyPropertyChangedBase, IMediaPlayer
    {
        public abstract IVideoView VideoView { get; set; }
        public virtual bool AutoAttachVideoView { get; set; } = true;
        public virtual VideoAspectMode VideoAspect { get; set; }
        public virtual bool ShowPlaybackControls { get; set; }

        private int _videoWidth;
        public int VideoWidth
        {
            get => _videoWidth;
            set => SetProperty(ref _videoWidth, value);
        }

        private int _videoHeight;
        public int VideoHeight
        {
            get => _videoHeight;
            set => SetProperty(ref _videoHeight, value);
        }

        public float VideoAspectRatio => VideoHeight == 0 ? 0 : (float)VideoWidth/VideoHeight;

        public abstract event BeforePlayingEventHandler BeforePlaying;
        public abstract event AfterPlayingEventHandler AfterPlaying;

        public abstract Task Pause();
        public abstract Task Play(IMediaItem mediaItem);
        public abstract Task Play();
        public abstract Task SeekTo(TimeSpan position);
        public abstract Task Stop();

        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
