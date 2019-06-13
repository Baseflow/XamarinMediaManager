using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Volume;

namespace MediaManager
{
    public class MediaManagerImplementation : MediaManagerBase //<MediaPlayer, Player>
    {
        public override IMediaPlayer MediaPlayer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IMediaExtractor MediaExtractor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IVolumeManager VolumeManager { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //public override MediaPlayerState State => throw new NotImplementedException();

        public override TimeSpan Position => throw new NotImplementedException();

        public override TimeSpan Duration => throw new NotImplementedException();

        public override float Speed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override RepeatMode RepeatMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override ShuffleMode ShuffleMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override INotificationManager NotificationManager { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override Task Pause()
        {
            throw new NotImplementedException();
        }

        public override Task Play(IMediaItem mediaItem)
        {
            throw new NotImplementedException();
        }

        public override Task<IMediaItem> Play(string uri)
        {
            throw new NotImplementedException();
        }

        public override Task Play(IEnumerable<IMediaItem> items)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
        {
            throw new NotImplementedException();
        }

        public override Task<IMediaItem> Play(FileInfo file)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo)
        {
            throw new NotImplementedException();
        }

        public override Task Play()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> PlayNext()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> PlayPrevious()
        {
            throw new NotImplementedException();
        }

        public override Task SeekTo(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public override Task StepBackward()
        {
            throw new NotImplementedException();
        }

        public override Task StepForward()
        {
            throw new NotImplementedException();
        }

        public override Task Stop()
        {
            throw new NotImplementedException();
        }
    }
}
