using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Platforms.Android;
using MediaManager.Playback;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager.Platforms.Android
{
    public class MediaBrowserManager
    {
        public MediaControllerCompat mediaController;
        private MediaBrowserCompat mediaBrowser;
        private ConnectionCallback mConnectionCallback;
        private MediaControllerCallback mMediaControllerCallback;

        //TODO: Make it possible to set context from app
        private Context _context;
        bool _isConnected = false;

        public async Task<bool> EnsureInitialized()
        {
            if (_isConnected)
                return true;

            _context = Application.Context;
            mMediaControllerCallback = new MediaControllerCallback();
            SubscriptionCallback subscriptionCallback = new SubscriptionCallback();

            // Connect a media browser just to get the media session token. There are other ways
            // this can be done, for example by sharing the session token directly.
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            mConnectionCallback = new ConnectionCallback();
            mConnectionCallback.OnConnectedImpl = () =>
            {
                mediaController = new MediaControllerCompat(_context, mediaBrowser.SessionToken);
                mediaController.RegisterCallback(mMediaControllerCallback);

                // Sync existing MediaSession state to the UI.
                mMediaControllerCallback.OnMetadataChanged(mediaController.Metadata);
                mMediaControllerCallback.OnPlaybackStateChanged(mediaController.PlaybackState);

                mediaBrowser.Subscribe(mediaBrowser.Root, subscriptionCallback);

                _isConnected = true;
                tcs.SetResult(_isConnected);
            };
            mConnectionCallback.OnConnectionFailedImpl = () =>
            {
                _isConnected = false;
                tcs.SetResult(_isConnected);
            };

            mediaBrowser = new MediaBrowserCompat(_context,
                new ComponentName(_context, Java.Lang.Class.FromType(typeof(AudioPlayerService))),
                mConnectionCallback, null);

            mediaBrowser.Connect();
            return _isConnected = await tcs.Task;
        }

        private class MediaControllerCallback : MediaControllerCompat.Callback
        {
            public override void OnPlaybackStateChanged(PlaybackStateCompat state)
            {
                base.OnPlaybackStateChanged(state);
            }

            public override void OnMetadataChanged(MediaMetadataCompat metadata)
            {
                base.OnMetadataChanged(metadata);
            }

            public override void OnSessionEvent(string @event, Bundle extras)
            {
                base.OnSessionEvent(@event, extras);
            }
        }

        class ConnectionCallback : MediaBrowserCompat.ConnectionCallback
        {
            public Action OnConnectedImpl { get; set; }

            public Action OnConnectionFailedImpl { get; set; }

            public Action OnConnectionSuspendedImpl { get; set; }

            public override void OnConnected()
            {
                OnConnectedImpl();
            }

            public override void OnConnectionFailed()
            {
                OnConnectionFailedImpl();
            }

            public override void OnConnectionSuspended()
            {
                OnConnectionSuspendedImpl();
            }
        }

        class SubscriptionCallback : MediaBrowserCompat.SubscriptionCallback
        {
            public Action<string, IList<MediaBrowserCompat.MediaItem>> OnChildrenLoadedImpl { get; set; }

            public Action<string> OnErrorImpl { get; set; }

            public override void OnChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children)
            {
                OnChildrenLoadedImpl(parentId, children);
            }

            public override void OnError(string id)
            {
                OnErrorImpl(id);
            }
        }
    }
}
