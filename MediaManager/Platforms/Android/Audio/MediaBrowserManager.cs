using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using MediaManager.Platforms.Android.Utils;

namespace MediaManager.Platforms.Android.Audio
{
    public class MediaBrowserManager
    {
        public MediaControllerCompat MediaController { get; set; }

        protected MediaBrowserCompat MediaBrowser { get; set; }
        protected MediaBrowserConnectionCallback MediaBrowserConnectionCallback { get; set; }
        protected MediaControllerCallback MediaControllerCallback { get; set; }
        protected MediaBrowserSubscriptionCallback MediaBrowserSubscriptionCallback { get; set; }

        protected bool IsInitialized { get; private set; } = false;
        protected Context Context { get; private set; }

        public MediaBrowserManager(Context context)
        {
            Context = context;
        }

        public async Task<bool> EnsureInitialized()
        {
            if (IsInitialized)
                return true;

            MediaControllerCallback = new MediaControllerCallback();
            MediaBrowserSubscriptionCallback = new MediaBrowserSubscriptionCallback();

            // Connect a media browser just to get the media session token. There are other ways
            // this can be done, for example by sharing the session token directly.
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            MediaBrowserConnectionCallback = new MediaBrowserConnectionCallback
            {
                OnConnectedImpl = () =>
                {
                    MediaControllerCallback.OnMetadataChangedImpl = metadata =>
                    {
                    };

                    MediaControllerCallback.OnPlaybackStateChangedImpl = state =>
                    {
                        CrossMediaManager.Current.OnStatusChanged(this, new Abstractions.EventArguments.StatusChangedEventArgs(state.ToMediaPlayerState()));
                    };

                    MediaControllerCallback.OnSessionEventChangedImpl = (string @event, Bundle extras) =>
                    {
                        //Do nothing for now
                    };

                    MediaController = new MediaControllerCompat(Context, MediaBrowser.SessionToken);
                    MediaController.RegisterCallback(MediaControllerCallback);

                    if (Context is Activity activity)
                        MediaControllerCompat.SetMediaController(activity, MediaController);

                    // Sync existing MediaSession state to the UI.
                    // The first time these events are fired, the metadata and playbackstate are null. 
                    MediaControllerCallback.OnMetadataChanged(MediaController.Metadata);
                    MediaControllerCallback.OnPlaybackStateChanged(MediaController.PlaybackState);

                    MediaBrowser.Subscribe(MediaBrowser.Root, MediaBrowserSubscriptionCallback);

                    IsInitialized = true;
                    tcs.SetResult(IsInitialized);
                },

                OnConnectionFailedImpl = () =>
                {
                    IsInitialized = false;
                    tcs.SetResult(IsInitialized);
                }
            };

            MediaBrowser = new MediaBrowserCompat(Context,
                new ComponentName(
                    Context,
                    Java.Lang.Class.FromType(typeof(MediaBrowserService))),
                    MediaBrowserConnectionCallback,
                    null);

            MediaBrowser.Connect();
            return IsInitialized = await tcs.Task;
        }
    }
}
