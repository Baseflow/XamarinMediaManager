using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager
{
    public class MediaManagerImplementation : IMediaManager
    {
        public MediaManagerImplementation()
        {
        }

        public IAudioPlayer AudioPlayer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IVideoPlayer VideoPlayer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IMediaQueue MediaQueue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IMediaNotificationManager MediaNotificationManager { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IMediaExtractor MediaExtractor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IVolumeManager VolumeManager { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IPlaybackController PlaybackController { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MediaPlayerStatus Status => throw new NotImplementedException();

        public TimeSpan Position => throw new NotImplementedException();

        public TimeSpan Duration => throw new NotImplementedException();

        public TimeSpan Buffered => throw new NotImplementedException();

        public Dictionary<string, string> RequestHeaders { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event MediaFileChangedEventHandler MediaFileChanged;
        public event MediaFileFailedEventHandler MediaFileFailed;
        public event StatusChangedEventHandler StatusChanged;
        public event PlayingChangedEventHandler PlayingChanged;
        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFinishedEventHandler MediaFinished;
        public event MediaFailedEventHandler MediaFailed;

        public Task Pause()
        {
            throw new NotImplementedException();
        }

        public Task Play(string url)
        {
            throw new NotImplementedException();
        }

        public Task Play(string url, MediaFileType fileType)
        {
            throw new NotImplementedException();
        }

        public Task Play(string url, MediaFileType fileType, ResourceAvailability availability)
        {
            throw new NotImplementedException();
        }

        public Task Play(IEnumerable<IMediaFile> mediaFiles)
        {
            throw new NotImplementedException();
        }

        public Task Play(IMediaFile mediaFile = null)
        {
            throw new NotImplementedException();
        }

        public Task PlayByPosition(int index)
        {
            throw new NotImplementedException();
        }

        public Task PlayNext()
        {
            throw new NotImplementedException();
        }

        public Task PlayPrevious()
        {
            throw new NotImplementedException();
        }

        public Task Seek(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public void SetOnBeforePlay(Func<IMediaFile, Task> beforePlay)
        {
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }
    }
}
