using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using MediaManager.Media;
using MediaManager.Playback;

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

        public MediaPlayerStatus Status => MediaBrowserManager?.PlaybackState?.Status ?? MediaPlayerStatus.stopped;

        public Dictionary<string, string> RequestHeaders { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public PlaybackManager(MediaManagerImplementation mediaManagerImplementation)
        {
            this.mediaManagerImplementation = mediaManagerImplementation;
        }

        public async Task Pause()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.mediaController.GetTransportControls().Pause();
        }

        public async Task Play()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.mediaController.GetTransportControls().Play();
        }

        public async Task Play(IMediaItem mediaItem)
        {
            await MediaBrowserManager.EnsureInitialized();
            var mediaUri = global::Android.Net.Uri.Parse(mediaItem.MetadataMediaUri);
            //MediaBrowserManager.mediaController.GetTransportControls().PlayFromUri(mediaUri, null);
            MediaBrowserManager.mediaController.GetTransportControls().PrepareFromUri(mediaUri, null);
            MediaBrowserManager.mediaController.GetTransportControls().Play();
        }

        public async Task PlayNext()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.mediaController.GetTransportControls().SkipToNext();
        }

        public async Task PlayPause()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.mediaController.GetTransportControls().SkipToNext();
        }

        public async Task PlayPrevious()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.mediaController.GetTransportControls().SkipToPrevious();
        }

        public async Task PlayPreviousOrSeekToStart()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.mediaController.GetTransportControls().SeekTo(0);
        }

        public async Task SeekTo(TimeSpan position)
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.mediaController.GetTransportControls().SeekTo((long)position.TotalMilliseconds);
        }

        public async Task SeekToStart()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.mediaController.GetTransportControls().SeekTo(0);
        }

        public async Task StepBackward()
        {
            throw new NotImplementedException();
        }

        public async Task StepForward()
        {
            throw new NotImplementedException();
        }

        public async Task Stop()
        {
            await MediaBrowserManager.EnsureInitialized();
            MediaBrowserManager.mediaController.GetTransportControls().Stop();
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
