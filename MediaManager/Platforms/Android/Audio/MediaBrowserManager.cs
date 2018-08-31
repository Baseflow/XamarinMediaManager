using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;

namespace MediaManager.Platforms.Android.Audio
{
    public class MediaBrowserManager
    {
        public MediaControllerCompat MediaController;
        protected MediaBrowserCompat _mediaBrowser;

        protected MediaBrowserConnectionCallback _connectionCallback;
        protected MediaControllerCallback _mediaControllerCallback;
        private MediaBrowserSubscriptionCallback _subscriptionCallback;

        private MediaManagerImplementation _mediaManagerImplementation;

        public MediaBrowserManager(MediaManagerImplementation mediaManagerImplementation)
        {
            _mediaManagerImplementation = mediaManagerImplementation;
        }

        public bool IsInitialized { get; private set; } = false;

        public async Task<bool> EnsureInitialized()
        {
            if (IsInitialized)
                return true;

            _mediaControllerCallback = new MediaControllerCallback();
            _subscriptionCallback = new MediaBrowserSubscriptionCallback();

            // Connect a media browser just to get the media session token. There are other ways
            // this can be done, for example by sharing the session token directly.
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            _connectionCallback = new MediaBrowserConnectionCallback
            {
                OnConnectedImpl = () =>
                {
                    _mediaControllerCallback.OnMetadataChangedImpl = metadata =>
                    {
                        //TODO: trigger change
                    };

                    _mediaControllerCallback.OnPlaybackStateChangedImpl = state =>
                    {
                        //PlaybackState = new Utils.PlaybackStateCompatExtension(state);
                    };

                    _mediaControllerCallback.OnSessionEventChangedImpl = (string @event, Bundle extras) =>
                    {
                        //Do nothing for now
                    };

                    MediaController = new MediaControllerCompat(_mediaManagerImplementation.Context, _mediaBrowser.SessionToken);
                    MediaController.RegisterCallback(_mediaControllerCallback);

                    if (_mediaManagerImplementation.Context is Activity activity)
                        MediaControllerCompat.SetMediaController(activity, MediaController);

                    // Sync existing MediaSession state to the UI.
                    // The first time these events are fired, the metadata and playbackstate are null. 
                    _mediaControllerCallback.OnMetadataChanged(MediaController.Metadata);
                    _mediaControllerCallback.OnPlaybackStateChanged(MediaController.PlaybackState);

                    _mediaBrowser.Subscribe(_mediaBrowser.Root, _subscriptionCallback);

                    IsInitialized = true;
                    tcs.SetResult(IsInitialized);
                },

                OnConnectionFailedImpl = () =>
                {
                    IsInitialized = false;
                    tcs.SetResult(IsInitialized);
                }
            };

            _mediaBrowser = new MediaBrowserCompat(_mediaManagerImplementation.Context,
                new ComponentName(
                    _mediaManagerImplementation.Context, 
                    Java.Lang.Class.FromType(typeof(MediaBrowserService))),
                    _connectionCallback,
                    null);

            _mediaBrowser.Connect();
            return IsInitialized = await tcs.Task;
        }
    }
}
