using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Platforms.Android.Audio;

namespace MediaManager.Platforms.Android
{
    public class PlaybackManager : IPlaybackManager
    {
        protected MediaManagerImplementation mediaManagerImplementation;

        public event PropertyChangedEventHandler PropertyChanged;

        protected MediaBrowserManager MediaBrowserManager => mediaManagerImplementation.MediaBrowserManager;

        public TimeSpan Position => MediaBrowserManager?.PlaybackState?.Position ?? TimeSpan.Zero;

        public TimeSpan Duration => TimeSpan.FromMilliseconds(MediaBrowserManager?.Metadata?.Duration ?? 0);

        public TimeSpan Buffered => MediaBrowserManager?.PlaybackState?.Buffered ?? TimeSpan.Zero;

        public MediaPlayerStatus Status => MediaBrowserManager?.PlaybackState?.Status ?? MediaPlayerStatus.Stopped;

        public Dictionary<string, string> RequestHeaders { get; set; }

        public PlaybackManager(MediaManagerImplementation mediaManagerImplementation)
        {
            this.mediaManagerImplementation = mediaManagerImplementation;
        }

        public async Task Pause()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().Pause();
        }

        public async Task Play()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().Play();
        }

        public async Task Play(IMediaItem mediaItem)
        {
            await MediaBrowserManager.EnsureInitialized();
            var mediaUri = global::Android.Net.Uri.Parse(mediaItem.MetadataMediaUri);
            MediaBrowserManager.MediaController.GetTransportControls().PlayFromUri(mediaUri, null);
        }

        public async Task PlayNext()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().SkipToNext();
        }

        public async Task PlayPrevious()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().SkipToPrevious();
        }

        public async Task PlayPreviousOrSeekToStart()
        {
            await MediaBrowserManager.EnsureInitialized();
            if (Position < TimeSpan.FromSeconds(3))
                MediaBrowserManager.MediaController.GetTransportControls().SkipToPrevious();
            else
                MediaBrowserManager.MediaController.GetTransportControls().SeekTo(0);
        }

        public async Task SeekTo(TimeSpan position)
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().SeekTo((long)position.TotalMilliseconds);
        }

        public async Task SeekToStart()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().SeekTo(0);
        }

        public async Task StepBackward()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().Rewind();
        }

        public async Task StepForward()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().FastForward();
        }

        public async Task Stop()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.MediaController.GetTransportControls().Stop();
        }

        public void ToggleRepeat()
        {
            MediaBrowserManager.MediaController.GetTransportControls().SetRepeatMode(0);
        }

        public void ToggleShuffle()
        {
            MediaBrowserManager.MediaController.GetTransportControls().SetShuffleMode(0);
        }
    }
}
