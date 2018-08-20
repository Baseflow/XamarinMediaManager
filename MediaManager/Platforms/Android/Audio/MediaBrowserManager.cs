using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;

namespace MediaManager.Platforms.Android
{
    public class MediaBrowserManager
    {
        public MediaControllerCompat mediaController;
        protected MediaBrowserCompat mediaBrowser;
        protected ConnectionCallback mConnectionCallback;
        protected MediaControllerCallback mMediaControllerCallback;
        private IMediaManager mediaManager;

        //TODO: Make it possible to set context from app
        protected Context _context { get; set; } = Application.Context;

        public bool IsInitialized { get; private set; } = false;

        public Utils.PlaybackStateCompatExtension PlaybackState { get; private set; }

        public Utils.MediaMetadataCompatExtension Metadata { get; internal set; }

        public MediaBrowserManager(IMediaManager mediaManager)
        {
            this.mediaManager = mediaManager;

            //make sure the service connection is initialized.
            EnsureInitialized().Wait();
        }

        public async Task<bool> EnsureInitialized()
        {
            if (IsInitialized)
                return true;

            mMediaControllerCallback = new MediaControllerCallback();
            SubscriptionCallback subscriptionCallback = new SubscriptionCallback();

            // Connect a media browser just to get the media session token. There are other ways
            // this can be done, for example by sharing the session token directly.
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            mConnectionCallback = new ConnectionCallback
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

                    mediaController = new MediaControllerCompat(_context, mediaBrowser.SessionToken);
                    mediaController.RegisterCallback(mMediaControllerCallback);

                    // Sync existing MediaSession state to the UI.
                    // The first time these events are fired, the metadata and playbackstate are null. 
                    //mMediaControllerCallback.OnMetadataChanged(mediaController.Metadata);
                    //mMediaControllerCallback.OnPlaybackStateChanged(mediaController.PlaybackState);

                    mediaBrowser.Subscribe(mediaBrowser.Root, subscriptionCallback);

                    ((AndroidMediaQueue)mediaManager.MediaQueue).AndroidQueue = mediaController.Queue;

                    IsInitialized = true;
                    tcs.SetResult(IsInitialized);
                },

                OnConnectionFailedImpl = () =>
                {
                    IsInitialized = false;
                    tcs.SetResult(IsInitialized);
                }
            };

            mediaBrowser = new MediaBrowserCompat(_context,
                new ComponentName(_context, Java.Lang.Class.FromType(typeof(MediaBrowserService))),
                mConnectionCallback, null);

            mediaBrowser.Connect();
            return IsInitialized = await tcs.Task;
        }

        public class MediaControllerCallback : MediaControllerCompat.Callback
        {
            public Action<PlaybackStateCompat> OnPlaybackStateChangedImpl { get; set; }
            public Action<MediaMetadataCompat> OnMetadataChangedImpl { get; set; }
            public Action<string, Bundle> OnSessionEventChangedImpl { get; set; }

            public override void OnPlaybackStateChanged(PlaybackStateCompat state)
            {
                base.OnPlaybackStateChanged(state);
                OnPlaybackStateChangedImpl?.Invoke(state);
            }

            public override void OnMetadataChanged(MediaMetadataCompat metadata)
            {
                base.OnMetadataChanged(metadata);
                OnMetadataChangedImpl?.Invoke(metadata);
            }

            public override void OnSessionEvent(string @event, Bundle extras)
            {
                base.OnSessionEvent(@event, extras);
                OnSessionEventChangedImpl?.Invoke(@event, extras);
            }
        }

        protected class ConnectionCallback : MediaBrowserCompat.ConnectionCallback
        {
            public Action OnConnectedImpl { get; set; }

            public Action OnConnectionFailedImpl { get; set; }

            public Action OnConnectionSuspendedImpl { get; set; }

            public override void OnConnected()
            {
                OnConnectedImpl?.Invoke();
            }

            public override void OnConnectionFailed()
            {
                OnConnectionFailedImpl?.Invoke();
            }

            public override void OnConnectionSuspended()
            {
                OnConnectionSuspendedImpl?.Invoke();
            }
        }

        protected class SubscriptionCallback : MediaBrowserCompat.SubscriptionCallback
        {
            public Action<string, IList<MediaBrowserCompat.MediaItem>> OnChildrenLoadedImpl { get; set; }

            public Action<string> OnErrorImpl { get; set; }

            public override void OnChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children)
            {
                OnChildrenLoadedImpl?.Invoke(parentId, children);
            }

            public override void OnError(string id)
            {
                OnErrorImpl?.Invoke(id);
            }
        }
    }
}
