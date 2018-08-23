using System;
using System.Threading.Tasks;
using MediaManager;
using MediaManager.Media;

namespace ElementPlayer.Core
{
    public class PlayerViewModel
    {
        private IMediaManager _mediaManager;

        public PlayerViewModel()
        {
            _mediaManager = CrossMediaManager.Current;
        }

        public async Task Play(IMediaItem item)
        {
            await _mediaManager.Play(item);
        }

        public MediaPlayerStatus Status => _mediaManager.Status;

        public TimeSpan Duration => _mediaManager.Duration;

        public TimeSpan Buffered => _mediaManager.Buffered;

        public TimeSpan Position => _mediaManager.Position;

        public async Task PlayPause()
        {
            await _mediaManager.PlayPause();
        }

        public async Task Pause()
        {
            await _mediaManager.Pause();
        }

        public async Task PlayNext()
        {
            await _mediaManager.PlayNext();
        }

        public async Task PlayPrevious()
        {
            await _mediaManager.PlayPrevious();
        }

        public async Task SeekTo(int totalMilliSeconds)
        {
            await _mediaManager.SeekTo(totalMilliSeconds);
        }

        public async Task<IMediaItem> CreateMediaItem(string url)
        {
            return await _mediaManager.MediaExtractor.CreateMediaItem(url);
        }

        public IMediaQueue MediaQueue
        {
            get
            {
                return _mediaManager.MediaQueue;
            }
        }
    }
}


