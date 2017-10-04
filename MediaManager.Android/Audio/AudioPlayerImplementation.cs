using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager.Audio
{
    public class AudioPlayerImplementation : Java.Lang.Object, IAudioPlayer
    {
        private MediaManagerImplementation _mediaManagerImplementation;

        public MediaBrowserCompat mediaBrowser { get; private set; }
        public MediaControllerCompat mediaController { get; private set; }
        public MediaBrowserConnectionCallback mediaBrowserConnectionCallback { get; private set; }
        public MediaControllerCallback mediaControllerCallback { get; private set; }

        public AudioPlayerImplementation(MediaManagerImplementation mediaManagerImplementation)
        {
            _mediaManagerImplementation = mediaManagerImplementation;
        }

        public async Task<bool> ConnectService()
        {
            if (mediaController != null)
                return true;

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            mediaBrowserConnectionCallback = new MediaBrowserConnectionCallback(_mediaManagerImplementation);
            mediaBrowserConnectionCallback.OnConnectedImpl = () => {
                mediaController = new MediaControllerCompat(_mediaManagerImplementation.Context, mediaBrowser.SessionToken);
                mediaControllerCallback = new MediaControllerCallback(_mediaManagerImplementation);
                mediaController.RegisterCallback(mediaControllerCallback);
                tcs.TrySetResult(true);
            };

            mediaBrowser = new MediaBrowserCompat(_mediaManagerImplementation.Context, new ComponentName(_mediaManagerImplementation.Context, Java.Lang.Class.FromType(typeof(AudioPlayerService))), mediaBrowserConnectionCallback, null);
            mediaBrowser.Connect();

            return await tcs.Task;
        }

        public PlaybackState State => throw new NotImplementedException();

        public TimeSpan Position => throw new NotImplementedException();

        public TimeSpan Duration => throw new NotImplementedException();

        public TimeSpan Buffered => throw new NotImplementedException();

        public Dictionary<string, string> RequestHeaders { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event StatusChangedEventHandler Status;
        public event PlayingChangedEventHandler Playing;
        public event BufferingChangedEventHandler Buffering;
        public event MediaFinishedEventHandler Finished;
        public event MediaFailedEventHandler Failed;

        public Task Pause()
        {
            return Task.CompletedTask;
        }

        public async Task Play(string url)
        {
            await ConnectService();
            mediaController.GetTransportControls().PlayFromUri(Android.Net.Uri.Parse(url), null);
        }

        public async Task Play(IMediaItem item)
        {
            await ConnectService();

            var test = new Android.OS.Bundle();
            //test.
            mediaController.GetTransportControls().PlayFromMediaId("test", test);
        }

        public async Task Seek(TimeSpan position)
        {
            await ConnectService();
        }

        public async Task Stop()
        {
            await ConnectService();
        }
    }
}
