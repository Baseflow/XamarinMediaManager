using System;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager
{
    public abstract class MediaManagerBase : IMediaManager
    {
        public MediaPlayerStatus Status => PlaybackManager.Status;
        public TimeSpan Duration => PlaybackManager.Duration;
        public TimeSpan Buffered => PlaybackManager.Buffered;
        public TimeSpan Position => PlaybackManager.Position;


        //public abstract IAudioPlayer AudioPlayer { get; set; }
        public abstract IVideoPlayer VideoPlayer { get; set; }
        public abstract INotificationManager NotificationManager { get; set; }

        public abstract IMediaExtractor MediaExtractor { get; set; }
        public abstract IVolumeManager VolumeManager { get; set; }

        protected IMediaQueue _mediaQueue;
        public virtual IMediaQueue MediaQueue
        {
            get
            {
                if (_mediaQueue == null)
                    _mediaQueue = new MediaQueue();

                return _mediaQueue;
            }
            set
            {
                _mediaQueue = value;
            }
        }

        public abstract IPlaybackManager PlaybackManager { get; set; }

        public IMediaLoader MediaLoader { get; set; }
    }
}
