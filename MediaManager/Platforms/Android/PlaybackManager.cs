using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Playback;

namespace MediaManager.Platforms.Android
{
    public class PlaybackManager : PlaybackManagerBase
    {
        private MediaManagerImplementation mediaManagerImplementation;

        public PlaybackManager(MediaManagerImplementation mediaManagerImplementation)
        {
            this.mediaManagerImplementation = mediaManagerImplementation;
        }

        public override IMediaPlayer CurrentMediaPlayer => mediaManagerImplementation.AudioPlayer;

        public override Task Pause()
        {
            throw new NotImplementedException();
        }

        public override Task Play()
        {
            mediaManagerImplementation.mediaController.GetTransportControls().Play();
            return Task.CompletedTask;
        }

        public override Task Play(IMediaItem mediaItem)
        {
            var mediaUri = global::Android.Net.Uri.Parse(mediaItem.MetadataMediaUri);
            mediaManagerImplementation.mediaController.GetTransportControls().PlayFromUri(mediaUri, null);
            return Task.CompletedTask;
        }

        public override Task PlayNext()
        {
            throw new NotImplementedException();
        }

        public override Task PlayPause()
        {
            throw new NotImplementedException();
        }

        public override Task PlayPrevious()
        {
            throw new NotImplementedException();
        }

        public override Task PlayPreviousOrSeekToStart()
        {
            throw new NotImplementedException();
        }

        public override Task SeekTo(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public override Task SeekToStart()
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

        public override void ToggleRepeat()
        {
            throw new NotImplementedException();
        }

        public override void ToggleShuffle()
        {
            throw new NotImplementedException();
        }
    }
}
