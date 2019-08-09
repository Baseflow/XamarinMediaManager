using System;
using System.Threading.Tasks;
using MediaManager.Library;
using MediaManager.Video;

namespace MediaManager.Player
{
    public abstract class MediaPlayerBase : NotifyPropertyChangedBase, IMediaPlayer
    {
        public abstract IVideoView VideoView { get; set; }

        private bool _autoAttachVideoView = true;
        public bool AutoAttachVideoView
        {
            get => _autoAttachVideoView;
            set => SetProperty(ref _autoAttachVideoView, value);
        }

        private VideoAspectMode _videoAspect;
        public VideoAspectMode VideoAspect
        {
            get => _videoAspect;
            set => SetProperty(ref _videoAspect, value);
        }

        private bool _showPlaybackControls;
        public bool ShowPlaybackControls
        {
            get => _showPlaybackControls;
            set => SetProperty(ref _showPlaybackControls, value);
        }

        private int _videoWidth;
        public int VideoWidth
        {
            get => _videoWidth;
            internal set => SetProperty(ref _videoWidth, value);
        }

        private int _videoHeight;

        public int VideoHeight
        {
            get => _videoHeight;
            internal set => SetProperty(ref _videoHeight, value);
        }

        public float VideoAspectRatio => VideoHeight == 0 ? 0 : (float)VideoWidth / VideoHeight;

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
