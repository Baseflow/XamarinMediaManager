using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using CoreMedia;
using Foundation;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Playback;

namespace MediaManager.Platforms.Apple.Media
{
    public abstract class AppleMediaPlayer : NSObject, IAudioPlayer<AVPlayer>
    {
        private NSObject DidFinishPlayingObserver;
        private NSObject ItemFailedToPlayToEndTimeObserver;
        private NSObject ErrorObserver;
        protected INotifyMediaManager MediaManager = CrossMediaManager.Current as INotifyMediaManager;

        public AppleMediaPlayer()
        {
        }

        private AVPlayer _player;
        public AVPlayer Player
        {
            get
            {
                if (this._player == null)
                {
                    this.Initialize();
                }

                return this._player;
            }
            set
            {
                this._player = value;
            }
        }

        private MediaPlayerState _state;
        public MediaPlayerState State
        {
            get { return _state; }
            private set
            {
                _state = value;
                MediaManager.OnStateChanged(this, new StateChangedEventArgs(_state));
            }
        }

        public event BeforePlayingEventHandler BeforePlaying;
        public event AfterPlayingEventHandler AfterPlaying;

        public virtual void Initialize()
        {
            Player = new AVPlayer();

            _state = MediaPlayerState.Stopped;

            DidFinishPlayingObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, DidFinishPlaying);
            ItemFailedToPlayToEndTimeObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.ItemFailedToPlayToEndTimeNotification, DidErrorOcurred);
            ErrorObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.NewErrorLogEntryNotification, DidErrorOcurred);

            // Watch the buffering status. If it changes, we may have to resume because the playing stopped because of bad network-conditions.
            MediaManager.BufferingChanged += (sender, e) =>
            {
                // If the player is ready to play, it's paused and the status is still on PLAYING, go on!
                if ((Player.Status == AVPlayerStatus.ReadyToPlay) && (Player.Rate == 0.0f) &&
                    (State == MediaPlayerState.Playing))
                    Player.Play();
            };
        }

        private void DidErrorOcurred(NSNotification obj)
        {
            throw new NotImplementedException();
        }

        private async void DidFinishPlaying(NSNotification obj)
        {
            if (RepeatMode == RepeatMode.One)
            {
                // Do not set the state to stopped, but just reiterate playing the element
                await Seek(new TimeSpan(0));
                return;
            }
            if (RepeatMode == RepeatMode.All)
            {
                throw new ArgumentException("Repeatmode all has not yet been implemented for iOS");
                // TODO: Implement the all repeat mode
                // Do not set the state to stopped, but just reiterate playing the element
                //await Seek(new TimeSpan(0));
                //return;
            }
            State = MediaPlayerState.Stopped;
        }

        public Task Pause()
        {
            Player.Pause();
            State = MediaPlayerState.Stopped;
            return Task.CompletedTask;
        }

        public Task Play(IMediaItem mediaItem)
        {
            var item = mediaItem.GetPlayerItem();

            Player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;
            Player.ReplaceCurrentItemWithPlayerItem(item);
            Player.Play();

            State = MediaPlayerState.Playing;
            return Task.CompletedTask;
        }

        public Task Play()
        {
            Player.Play();
            State = MediaPlayerState.Playing;
            return Task.CompletedTask;
        }

        public async Task Seek(TimeSpan position)
        {
            await Player.SeekAsync(CMTime.FromSeconds(position.TotalSeconds, 1));
        }

        public Task Stop()
        {
            Player.Pause();
            State = MediaPlayerState.Stopped;
            return Task.CompletedTask;
        }

        public RepeatMode RepeatMode { get; set; } = RepeatMode.Off;
    }
}
