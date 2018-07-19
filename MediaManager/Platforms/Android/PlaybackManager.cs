using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Playback;

namespace MediaManager.Platforms.Android
{
    public class PlaybackManager : IPlaybackManager //PlaybackManagerBase
    {
        protected MediaManagerImplementation mediaManagerImplementation;
        protected MediaBrowserManager mediaBrowserManager => mediaManagerImplementation.MediaBrowserManager;

        public TimeSpan Position => throw new NotImplementedException();

        public TimeSpan Duration => throw new NotImplementedException();

        public TimeSpan Buffered => throw new NotImplementedException();

        public PlaybackManager(MediaManagerImplementation mediaManagerImplementation)
        {
            this.mediaManagerImplementation = mediaManagerImplementation;
        }

        //public override IMediaPlayer CurrentMediaPlayer => mediaManagerImplementation.AudioPlayer;

        public Task Pause()
        {
            throw new NotImplementedException();
        }

        public async Task Play()
        {
            await mediaBrowserManager.EnsureInitialized();
            mediaBrowserManager.mediaController.GetTransportControls().Play();
        }

        public async Task Play(IMediaItem mediaItem)
        {
            await mediaBrowserManager.EnsureInitialized();
            var mediaUri = global::Android.Net.Uri.Parse(mediaItem.MetadataMediaUri);
            mediaBrowserManager.mediaController.GetTransportControls().PlayFromUri(mediaUri, null);
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

        public Task SeekTo(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public Task SeekToStart()
        {
            throw new NotImplementedException();
        }

        public Task StepBackward()
        {
            throw new NotImplementedException();
        }

        public Task StepForward()
        {
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }

        public void ToggleRepeat()
        {
            throw new NotImplementedException();
        }

        public void ToggleShuffle()
        {
            throw new NotImplementedException();
        }
    }
}
