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
        public MediaControllerCompat mediaController;
        protected MediaBrowserCompat mediaBrowser;

        protected MediaBrowserConnectionCallback mConnectionCallback;
        protected MediaControllerCallback mMediaControllerCallback;
        private MediaBrowserSubscriptionCallback subscriptionCallback;

        private MediaManagerImplementation mediaManagerImplementation;

        public MediaBrowserManager(MediaManagerImplementation mediaManagerImplementation)
        {
            this.mediaManagerImplementation = mediaManagerImplementation;
        }

        public bool IsInitialized { get; private set; } = false;

        public Utils.PlaybackStateCompatExtension PlaybackState { get; private set; }

        public Utils.MediaMetadataCompatExtension Metadata { get; internal set; }

        public async Task<bool> EnsureInitialized()
        {
            if (IsInitialized)
                return true;

            mMediaControllerCallback = new MediaControllerCallback();
            subscriptionCallback = new MediaBrowserSubscriptionCallback();

            // Connect a media browser just to get the media session token. There are other ways
            // this can be done, for example by sharing the session token directly.
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            mConnectionCallback = new MediaBrowserConnectionCallback
            {
                OnConnectedImpl = () =>
                {
                    mMediaControllerCallback.OnMetadataChangedImpl = metadata =>
                    {
                        Metadata = new Utils.MediaMetadataCompatExtension(metadata);
                        ///TODO: Implement callback.
                    };

                    mMediaControllerCallback.OnPlaybackStateChangedImpl = state =>
                    {
                        PlaybackState = new Utils.PlaybackStateCompatExtension(state);
                    };

                    mMediaControllerCallback.OnSessionEventChangedImpl = (string @event, Bundle extras) =>
                    {
                        //Do nothing for now
                    };

                    mediaController = new MediaControllerCompat(mediaManagerImplementation.Context, mediaBrowser.SessionToken);
                    mediaController.RegisterCallback(mMediaControllerCallback);

                    if (mediaManagerImplementation.Context is Activity activity)
                        MediaControllerCompat.SetMediaController(activity, mediaController);

                    // Sync existing MediaSession state to the UI.
                    // The first time these events are fired, the metadata and playbackstate are null. 
                    //mMediaControllerCallback.OnMetadataChanged(mediaController.Metadata);
                    //mMediaControllerCallback.OnPlaybackStateChanged(mediaController.PlaybackState);

                    mediaBrowser.Subscribe(mediaBrowser.Root, subscriptionCallback);

                    //((AndroidMediaQueue)mediaManagerImplementation.MediaQueue).SetAndroidQueue(mediaController.Queue);

                    IsInitialized = true;
                    tcs.SetResult(IsInitialized);
                },

                OnConnectionFailedImpl = () =>
                {
                    IsInitialized = false;
                    tcs.SetResult(IsInitialized);
                }
            };

            mediaBrowser = new MediaBrowserCompat(mediaManagerImplementation.Context,
                new ComponentName(
                    mediaManagerImplementation.Context, 
                    Java.Lang.Class.FromType(typeof(MediaBrowserService))),
                    mConnectionCallback,
                    null);

            mediaBrowser.Connect();
            return IsInitialized = await tcs.Task;
        }
    }
}
