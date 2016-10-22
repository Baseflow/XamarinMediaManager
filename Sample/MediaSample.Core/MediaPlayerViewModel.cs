using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions;

namespace MediaManager.Sample.Core
{
    public class MediaPlayerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly IMediaManager mediaPlayer;
        public IMediaManager MediaPlayer
        {
            get
            {
                return mediaPlayer;
            }
        }

        private readonly IMediaQueue queue;
        public IMediaQueue Queue
        {
            get
            {
                return queue;
            }
        }

        public IMediaFile CurrentTrack
        {
            get
            {
                return queue.Current;
            }
        }

        public int Duration
        {
            get
            {
                return mediaPlayer.Duration > 0 ? mediaPlayer.Duration : 0;
            }
        }

        private bool _isSeeking = false;

        public bool IsSeeking
        {
            get
            {
                return _isSeeking;
            }
            set
            {
                // Put into an action so we can await the seek-command before we update the value. Prevents jumping of the progress-bar.
                var a = new Action(async () =>
                    {
                        // When disable user-seeking, update the position with the position-value
                        if (value == false)
                        {
                            await mediaPlayer.Seek(Position);
                        }

                        _isSeeking = value;
                    });
                a.Invoke();
            }
        }

        private int _position;

        public int Position
        {
            get
            {
                if (IsSeeking)
                    return _position;

                return mediaPlayer.Position > 0 ? mediaPlayer.Position : 0;
            }
            set
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
            }
        }

        public int Downloaded
        {
            get
            {
                return mediaPlayer.Buffered;
            }
        }

        public Boolean IsPlaying
        {
            get
            {
                return mediaPlayer.Status == PlayerStatus.PLAYING || mediaPlayer.Status == PlayerStatus.BUFFERING;
            }
        }

        public PlayerStatus Status
        {
            get
            {
                return mediaPlayer.Status;
            }
        }

        public object Cover
        {
            get
            {
                return mediaPlayer.Cover;
            }
        }

        public string PlayingText
        {
            get
            {
                return string.Format("Playing: {0} of {1}", (Queue.Index + 1).ToString(), Queue.Count.ToString());
            }
        }

        public MediaPlayerViewModel()
        {
            mediaPlayer = CrossMediaManager.Current;
            queue = new MediaQueue();

            mediaPlayer.StatusChanged -= OnStatusChanged;
            mediaPlayer.StatusChanged += OnStatusChanged;
            mediaPlayer.Playing -= OnPlaying;
            mediaPlayer.Playing += OnPlaying;
            mediaPlayer.Buffering -= OnBuffering;
            mediaPlayer.Buffering += OnBuffering;
            mediaPlayer.CoverReloaded -= OnCoverReloaded;
            mediaPlayer.CoverReloaded += OnCoverReloaded;

            queue.PropertyChanged -= OnQueuePropertyChanged;
            queue.PropertyChanged += OnQueuePropertyChanged;
        }

        private void OnQueuePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Index")
            {
                //RaiseAllPropertiesChanged();
            }
            else if (e.PropertyName == "Current")
            {
                OnPropertyChanged(nameof(CurrentTrack));
            }
            else if (e.PropertyName == "Count")
            {
                OnPropertyChanged(nameof(PlayingText));
            }
        }

        private void OnPlaying(object sender, EventArgs e)
        {
            if (!IsSeeking)
            {
                // TODO: Please kick that one out here when we have true forwarding of the triggers the player fires.
                OnPropertyChanged(nameof(Duration));
                OnPropertyChanged(nameof(Position));
            }
        }

        private void OnBuffering(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Downloaded));
        }

        private void OnStatusChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsPlaying));
            OnPropertyChanged(nameof(Duration));
            OnPropertyChanged(nameof(Position));
            OnPropertyChanged(nameof(Status));
        }

        private void OnCoverReloaded(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Cover));
        }

        public string GetFormattedTime(int value)
        {
            var span = TimeSpan.FromMilliseconds(value);
            if (span.Hours > 0)
            {
                return string.Format("{0}:{1:00}:{2:00}", (int)span.TotalHours, span.Minutes, span.Seconds);
            }
            else
            {
                return string.Format("{0}:{1:00}", (int)span.Minutes, span.Seconds);
            }
        }
    }
}
