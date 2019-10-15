using System;
using System.Threading.Tasks;
using MediaManager.Library;
using MediaManager.Video;

namespace MediaManager.Player
{
    public abstract class MediaPlayerBase : NotifyPropertyChangedBase, IMediaPlayer
    {
        public abstract IVideoView VideoView { get; set; }

        protected bool _autoAttachVideoView = true;
        public virtual bool AutoAttachVideoView
        {
            get => _autoAttachVideoView;
            set => SetProperty(ref _autoAttachVideoView, value);
        }

        protected virtual void UpdateVideoView()
        {
            UpdateVideoAspect(VideoAspect);
            UpdateShowPlaybackControls(ShowPlaybackControls);
            UpdateVideoPlaceholder(VideoPlaceholder);
        }

        protected VideoAspectMode _videoAspect;
        public virtual VideoAspectMode VideoAspect
        {
            get => _videoAspect;
            set
            {
                if (SetProperty(ref _videoAspect, value))
                    UpdateVideoAspect(value);
            }
        }

        public abstract void UpdateVideoAspect(VideoAspectMode videoAspectMode);

        protected bool _showPlaybackControls = false;
        public virtual bool ShowPlaybackControls
        {
            get => _showPlaybackControls;
            set
            {
                if (SetProperty(ref _showPlaybackControls, value))
                    UpdateShowPlaybackControls(value);
            }
        }

        public abstract void UpdateShowPlaybackControls(bool showPlaybackControls);

        protected int _videoWidth;
        public virtual int VideoWidth
        {
            get => _videoWidth;
            internal set => SetProperty(ref _videoWidth, value, () => OnPropertyChanged(nameof(VideoAspectRatio)));
        }

        protected int _videoHeight;

        public virtual int VideoHeight
        {
            get => _videoHeight;
            internal set => SetProperty(ref _videoHeight, value, () => OnPropertyChanged(nameof(VideoAspectRatio)));
        }

        public virtual float VideoAspectRatio => VideoHeight == 0 ? 0 : (float)VideoWidth / VideoHeight;

        private object _videoPlaceholder;
        public virtual object VideoPlaceholder
        {
            get => _videoPlaceholder;
            set
            {
                if (SetProperty(ref _videoPlaceholder, value))
                    UpdateVideoPlaceholder(value);
            }
        }

        public abstract void UpdateVideoPlaceholder(object value);

        public abstract event BeforePlayingEventHandler BeforePlaying;
        public abstract event AfterPlayingEventHandler AfterPlaying;

        public abstract Task Pause();
        public abstract Task Play(IMediaItem mediaItem);
        public abstract Task Play(IMediaItem mediaItem, TimeSpan startAt, TimeSpan? stopAt = null);
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
