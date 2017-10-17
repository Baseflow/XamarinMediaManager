using System;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager.Abstractions.Implementations
{
    public class PlaybackController : IPlaybackController
    {
        private MediaManagerBase mediaManagerBase;

        public PlaybackController(MediaManagerBase mediaManagerBase)
        {
            this.mediaManagerBase = mediaManagerBase;
        }

        public Task Pause()
        {
            throw new NotImplementedException();
        }

        public Task Play()
        {
            throw new NotImplementedException();
        }

        public Task PlayFromQueueByIndex(int index)
        {
            throw new NotImplementedException();
        }

        public Task PlayFromQueueByMediaItem(IMediaItem file)
        {
            throw new NotImplementedException();
        }

        public Task PlayNext()
        {
            throw new NotImplementedException();
        }

        public Task PlayPause()
        {
            throw new NotImplementedException();
        }

        public Task PlayPrevious()
        {
            throw new NotImplementedException();
        }

        public Task PlayPreviousOrSeekToStart()
        {
            throw new NotImplementedException();
        }

        public Task SeekBackward(TimeSpan? time = null)
        {
            throw new NotImplementedException();
        }

        public Task SeekForward(TimeSpan? time = null)
        {
            throw new NotImplementedException();
        }

        public Task SeekTo(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public Task SeekToStart()
        {
            throw new NotImplementedException();
        }

        public void SetRating()
        {
            throw new NotImplementedException();
        }

        public void SetRepeatMode(RepeatMode type)
        {
            throw new NotImplementedException();
        }

        public void SetShuffleMode(ShuffleMode type)
        {
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }

        /*private readonly IMediaManager _mediaManager;

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
            var status = _mediaManager.Status;

            var isPaused = status == PlaybackState.Paused;
            var isStopped = status == PlaybackState.Stopped;

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

        public virtual async Task StepForward()
        {
            var destination = PositionSeconds + StepSeconds;

            await SeekTo(destination);
        }

        public virtual async Task StepBackward()
        {
            var destination = PositionSeconds - StepSeconds;

            await SeekTo(destination);
        }

        public virtual async Task SeekTo(double seconds)
        {
            if (_mediaManager.Duration.TotalSeconds < seconds)
            {
                seconds = _mediaManager.Duration.TotalSeconds;
            } else if (seconds < 0)
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
        }*/
    }
}