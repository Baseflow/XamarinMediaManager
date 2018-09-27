using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager
{
    public abstract class MediaManagerBase : IMediaManager, INotifyMediaManager
    {
        public MediaManagerBase()
        {
            Timer.AutoReset = true;
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }
        
        public bool IsInitialized { get; protected set; }

        public abstract IAudioPlayer AudioPlayer { get; set; }
        public abstract IVideoPlayer VideoPlayer { get; set; }
        public abstract IMediaExtractor MediaExtractor { get; set; }
        public abstract IVolumeManager VolumeManager { get; set; }

        private IMediaQueue _mediaQueue;
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

        public abstract MediaPlayerState State { get; }
        public abstract TimeSpan Position { get; }
        public abstract TimeSpan Duration { get; }
        public abstract TimeSpan Buffered { get; }
        public abstract float Speed { get; set; }

        public abstract Task Pause();
        public abstract Task Play(IMediaItem mediaItem);
        public abstract Task<IMediaItem> Play(string uri);
        public abstract Task Play(IEnumerable<IMediaItem> items);
        public abstract Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items);
        public abstract Task<IMediaItem> Play(FileInfo file);
        public abstract Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo);
        public abstract Task Play();
        public abstract Task PlayNext();
        public abstract Task PlayPrevious();
        public abstract Task SeekTo(TimeSpan position);
        public abstract Task SeekToStart();
        public abstract Task StepBackward();
        public abstract Task StepForward();
        public abstract Task Stop();
        public abstract void ToggleRepeat();
        public abstract void ToggleShuffle();

        public Timer Timer { get; } = new Timer(1000);
        public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event StateChangedEventHandler StateChanged;
        public event PlayingChangedEventHandler PlayingChanged;
        public event BufferingChangedEventHandler BufferingChanged;

        public event MediaItemFinishedEventHandler MediaItemFinished;
        public event MediaItemChangedEventHandler MediaItemChanged;
        public event MediaItemFailedEventHandler MediaItemFailed;

        public void OnBufferingChanged(object sender, BufferingChangedEventArgs e) => BufferingChanged?.Invoke(sender, e);
        public void OnMediaItemChanged(object sender, MediaItemEventArgs e) => MediaItemChanged?.Invoke(sender, e);
        public void OnMediaItemFailed(object sender, MediaItemFailedEventArgs e) => MediaItemFailed?.Invoke(sender, e);
        public void OnMediaItemFinished(object sender, MediaItemEventArgs e) => MediaItemFinished?.Invoke(sender, e);
        public void OnPlayingChanged(object sender, PlayingChangedEventArgs e) => PlayingChanged?.Invoke(sender, e);
        public void OnStateChanged(object sender, StateChangedEventArgs e) => StateChanged?.Invoke(sender, e);

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsInitialized)
                return;

            if (this.IsPlaying())
            {
                OnPlayingChanged(this, new PlayingChangedEventArgs(Position, Duration));
            }
            if(this.IsBuffering())
            {
                OnBufferingChanged(this, new BufferingChangedEventArgs(Buffered));
            }
        }

        public abstract void Init();
    }
}
