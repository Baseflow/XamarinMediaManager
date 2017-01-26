using System;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager.Abstractions.Implementations
{
    public class PlaybackController: IPlaybackController
    {
        private readonly IMediaManager _mediaManager;

        private const int StepSeconds = 10;

        private IMediaQueue Queue => _mediaManager.MediaQueue;

        private double PositionSeconds => _mediaManager.Position.TotalSeconds;

        public PlaybackController(IMediaManager mediaManager)
        {
            _mediaManager = mediaManager;
        }

        public async Task PlayPause()
        {
            var status = _mediaManager.Status;

            var isPaused = status == MediaPlayerStatus.Paused;
            var isStopped = status == MediaPlayerStatus.Stopped;

            if (isPaused || isStopped)
            {
                await Play();
            }
            else
            {
                await Pause();
            }
        }

        public async Task Play()
        {
            await _mediaManager.Play();
        }

        public async Task Pause()
        {
            await _mediaManager.Pause();
        }

        public async Task Stop()
        {
            await _mediaManager.Stop();
        }

        public async Task PlayPreviousOrSeekToStart()
        {
            if (PositionSeconds > 3)
            {
                await SeekToStart();
            }
            else
            {
                await PlayPrevious();
            }
        }

        public async Task PlayNext()
        {
            await _mediaManager.PlayNext();
        }

        public async Task PlayPrevious()
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

        public async Task SeekToStart()
        {
            await SeekTo(0);
        }

        public async Task StepForward()
        {
            var destination = PositionSeconds + StepSeconds;

            await SeekTo(destination);
        }

        public async Task StepBackward()
        {
            var destination = PositionSeconds - StepSeconds;

            await SeekTo(destination);
        }

        public async Task SeekTo(double seconds)
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

        public void ToggleRepeat()
        {
            switch (Queue.Repeat)
            {
                case RepeatType.None:
                    Queue.Repeat = RepeatType.RepeatOne;
                    break;
                case RepeatType.RepeatOne:
                    Queue.Repeat = RepeatType.RepeatAll;
                    break;
                case RepeatType.RepeatAll:
                    Queue.Repeat = RepeatType.None;
                    break;
            }
        }

        public void ToggleShuffle()
        {
            Queue.IsShuffled = !Queue.IsShuffled;
        }
    }
}