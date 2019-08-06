using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Video;

namespace MediaManager.Player
{
    public abstract class MediaPlayerBase
    {
        public IVideoView VideoView { get; set; }
        public bool AutoAttachVideoView { get; set; } = true;
        public VideoAspectMode VideoAspect { get; set; }
        public bool ShowPlaybackControls { get; set; }

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
