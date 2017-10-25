using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager.Abstractions.Implementations
{
    public class PlaybackController : IPlaybackController
    {
        private readonly IMediaManager _mediaManager;

        public virtual double StepSeconds => 10;

        public virtual double SeekToStartTreshold => 3;

        private IMediaQueue Queue => _mediaManager.MediaQueue;

        private double PositionSeconds => _mediaManager.Position.TotalSeconds;

        public PlaybackController(IMediaManager mediaManager)
        {
            _mediaManager = mediaManager;
        }

        public virtual async Task PlayPause()
        {
            var state = _mediaManager.State;

            var isPaused = state == PlaybackState.Paused;
            var isStopped = state == PlaybackState.Stopped;

            if (isPaused || isStopped)
            {
                await Play();
            }
            else
            {
                await Pause();
            }
        }

        public virtual async Task Play()
        {
            await _mediaManager.Play();
        }

        public virtual async Task Pause()
        {
            await _mediaManager.Pause();
        }

        public virtual async Task Stop()
        {
            await _mediaManager.Stop();
        }

        public virtual async Task PlayPreviousOrSeekToStart()
        {
            if (PositionSeconds > SeekToStartTreshold)
            {
                await SeekToStart();
            }
            else
            {
                await PlayPrevious();
            }
        }

        public virtual async Task PlayNext()
        {
            await _mediaManager.PlayNext();
        }

        public async Task PlayFromQueueByIndex(int index)
        {
            //TODO: PlayFromQueueByIndex
        }

        public async Task PlayFromQueueByMediaItem(IMediaItem file)
        {
            //TODO: PlayFromQueueByMediaItem
        }

        public virtual async Task PlayPrevious()
        {
            if (Queue.HasPrevious())
            {
                await _mediaManager.PlayPrevious();
            }
            else
            {
                await SeekToStart();
            }
        }

        public virtual async Task SeekToStart()
        {
            await SeekTo(0);
        }

        public async Task SeekForward(TimeSpan? time = null)
        {
            if (time != null) await StepForward(time.Value.TotalSeconds);
        }

        public async Task SeekBackward(TimeSpan? time = null)
        {
            if (time != null) await StepBackward(time.Value.TotalSeconds);
        }

        public async Task SeekTo(TimeSpan position)
        {
            await SeekTo(position.TotalSeconds);
        }

        public void SetRepeatMode(RepeatMode type)
        {
            ToggleRepeat();
        }

        public void SetShuffleMode(ShuffleMode type)
        {
            ToggleShuffle();
        }

        public void SetRating()
        {
            //TODO: SetRating
        }

        public virtual async Task StepForward(double stepSeconds)
        {
            var destination = PositionSeconds + stepSeconds;

            await SeekTo(destination);
        }

        public virtual async Task StepBackward(double stepSeconds)
        {
            var destination = PositionSeconds - stepSeconds;

            await SeekTo(destination);
        }

        public virtual async Task SeekTo(double seconds)
        {
            if (_mediaManager.Duration.TotalSeconds < seconds)
            {
                seconds = _mediaManager.Duration.TotalSeconds;
            }
            else if (seconds < 0)
            {
                seconds = 0;
            }

            var position = TimeSpan.FromSeconds(seconds);

            await _mediaManager.Seek(position);
        }

        public virtual void ToggleRepeat()
        {
            switch (Queue.Repeat)
            {
                case RepeatMode.None:
                    Queue.Repeat = RepeatMode.RepeatAll;
                    break;
                case RepeatMode.RepeatAll:
                    Queue.Repeat = RepeatMode.RepeatOne;
                    break;
                case RepeatMode.RepeatOne:
                    Queue.Repeat = RepeatMode.None;
                    break;
            }
        }

        public virtual void ToggleShuffle()
        {
            Queue.IsShuffled = !Queue.IsShuffled;
        }
    }
}