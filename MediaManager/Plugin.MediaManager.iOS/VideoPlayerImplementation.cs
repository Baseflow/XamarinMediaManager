using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class VideoPlayerImplementation : IVideoPlayer
    {
        public TimeSpan Buffered
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TimeSpan Duration
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TimeSpan Position
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public MediaPlayerStatus Status
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFailedEventHandler MediaFailed;
        public event MediaFileFailedEventHandler MediaFileFailed;
        public event MediaFileChangedEventHandler MediaFileChanged;
        public event MediaFinishedEventHandler MediaFinished;
        public event PlayingChangedEventHandler PlayingChanged;
        public event StatusChangedEventHandler StatusChanged;

        public Task Pause()
        {
            throw new NotImplementedException();
        }

        public Task Play(IEnumerable<IMediaFile> mediaFiles)
        {
            throw new NotImplementedException();
        }

        public Task Play(IMediaFile mediaFile)
        {
            throw new NotImplementedException();
        }

        public Task Play(string url, MediaFileType fileType)
        {
            throw new NotImplementedException();
        }

        public Task PlayPause()
        {
            throw new NotImplementedException();
        }

        public Task Seek(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }

        public IVideoSurface RenderSurface { get; set; }
        public void SetVideoSurface(IVideoSurface videoSurface)
        {
            throw new NotImplementedException();
        }
    }
}
