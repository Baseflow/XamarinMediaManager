using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using MediaManager.Platforms.Android.Playback;

namespace MediaManager.Platforms.Android.MediaSession
{
    public class MediaBrowserManager
    {
        private TaskCompletionSource<bool> _tcs;

        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;

        //public MediaControllerCompat MediaController { get; set; }
        protected MediaBrowserCompat MediaBrowser { get; set; }
        protected MediaBrowserConnectionCallback MediaBrowserConnectionCallback { get; set; }
        protected MediaControllerCallback MediaControllerCallback { get; set; }
        protected MediaBrowserSubscriptionCallback MediaBrowserSubscriptionCallback { get; set; }
        public static Java.Lang.Class ServiceType { get; set; } = Java.Lang.Class.FromType(typeof(MediaBrowserService));

        protected bool IsConnected { get; private set; } = false;
        protected Context Context => MediaManager.Context;

        public MediaBrowserManager()
        {
        }

        public async Task<bool> Init()
        {
            if (_tcs?.Task != null && !_tcs.Task.IsCompleted)
                return await _tcs.Task;
            else if (IsConnected)
                return true;

            _tcs = new TaskCompletionSource<bool>();

            if (MediaBrowser == null)
            {
                MediaControllerCallback = new MediaControllerCallback()
                {
                    OnMetadataChangedImpl = metadata =>
                    {
                        //var test = metadata;
                    },
                    OnPlaybackStateChangedImpl = state =>
                    {
                        MediaManager.State = state.ToMediaPlayerState();
                    },
                    OnSessionEventChangedImpl = (string @event, Bundle extras) =>
                    {
                        //Do nothing for now
                    },
                    OnSessionDestroyedImpl = () =>
                    {
                        MediaBrowserConnectionCallback.OnConnectionSuspended();
                        MediaBrowserConnectionCallback.Dispose();
                        MediaBrowserConnectionCallback = null;

                        MediaBrowser.Unsubscribe(MediaBrowser.Root, MediaBrowserSubscriptionCallback);
                        MediaBrowserSubscriptionCallback.Dispose();
                        MediaBrowserSubscriptionCallback = null;

                        MediaBrowser.Disconnect();
                        MediaBrowser.Dispose();
                        MediaBrowser = null;

                        MediaManager.MediaController.UnregisterCallback(MediaControllerCallback);
                        MediaControllerCallback.Dispose();
                        MediaControllerCallback = null;

                        MediaManager.MediaController.Dispose();
                        MediaManager.MediaController = null;
                        IsConnected = false;
                    },
                    BinderDiedImpl = () =>
                    {
                        //Do nothing for now
                    },
                    OnSessionReadyImpl = () =>
                    {
                        //Do nothing for now
                    }
                };
                MediaBrowserSubscriptionCallback = new MediaBrowserSubscriptionCallback();

                // Connect a media browser just to get the media session token. There are other ways
                // this can be done, for example by sharing the session token directly.
                //TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                MediaBrowserConnectionCallback = new MediaBrowserConnectionCallback
                {
                    OnConnectedImpl = () =>
                    {
                        var mediaController = MediaManager.MediaController = new MediaControllerCompat(Context.ApplicationContext, MediaBrowser.SessionToken);
                        mediaController.RegisterCallback(MediaControllerCallback);

                        if (Context is Activity activity)
                            MediaControllerCompat.SetMediaController(activity, mediaController);

                        // Sync existing MediaSession state to the UI.
                        // The first time these events are fired, the metadata and playbackstate are null. 
                        MediaControllerCallback.OnMetadataChanged(mediaController.Metadata);
                        MediaControllerCallback.OnPlaybackStateChanged(mediaController.PlaybackState);

                        MediaBrowser.Subscribe(MediaBrowser.Root, MediaBrowserSubscriptionCallback);

                        IsConnected = true;
                        _tcs.SetResult(IsConnected);
                        _tcs = null;
                    },
                    OnConnectionFailedImpl = () =>
                    {
                        IsConnected = false;
                        _tcs.SetResult(IsConnected);
                        _tcs = null;
                    },
                    OnConnectionSuspendedImpl = () =>
                    {
                        IsConnected = false;
                    }
                };

                MediaBrowser = new MediaBrowserCompat(Context.ApplicationContext,
                    new ComponentName(
                        Context.ApplicationContext,
                        ServiceType),
                        MediaBrowserConnectionCallback,
                        null);
            }

            if (!IsConnected)
            {
                MediaBrowser.Connect();
            }

            return await _tcs.Task;
        }
    }
}
