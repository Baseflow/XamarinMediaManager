using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Source;
using MediaManager.Platforms.Android.Media;

namespace MediaManager.Platforms.Android.Player
{
    public class MediaSessionConnectorPlaybackPreparer : Java.Lang.Object, MediaSessionConnector.IPlaybackPreparer
    {
        protected IExoPlayer _player;
        protected ConcatenatingMediaSource _mediaSource;
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;

        public MediaSessionConnectorPlaybackPreparer(IExoPlayer player, ConcatenatingMediaSource mediaSource)
        {
            _player = player;
            _mediaSource = mediaSource;
        }

        protected MediaSessionConnectorPlaybackPreparer(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        //public long SupportedPrepareActions => MediaSessionConnector.IPlaybackPreparer.Actions;
        public long SupportedPrepareActions =>
            PlaybackStateCompat.ActionPrepare |
            PlaybackStateCompat.ActionPrepareFromMediaId |
            PlaybackStateCompat.ActionPrepareFromSearch |
            PlaybackStateCompat.ActionPrepareFromUri |
            PlaybackStateCompat.ActionPlayFromMediaId |
            PlaybackStateCompat.ActionPlayFromSearch |
            PlaybackStateCompat.ActionPlayFromUri;

        public bool OnCommand(IPlayer p0, IControlDispatcher p1, string p2, Bundle p3, ResultReceiver p4)
        {
            return false;
        }

        public void OnPrepare(bool p0)
        {
            // _mediaSource is filled through the QueueDataAdapter
            _player.Prepare(_mediaSource);

            //Only in case of Prepare set PlayWhenReady to true because we use this to load in the whole queue
            _player.PlayWhenReady = MediaManager.AutoPlay;
        }

        public void OnPrepareFromMediaId(string p0, bool p1, Bundle p2)
        {
            _mediaSource.Clear();
            var windowIndex = 0;
            foreach (var mediaItem in MediaManager.Queue)
            {
                if (mediaItem.Id == p0)
                    windowIndex = MediaManager.Queue.IndexOf(mediaItem);

                _mediaSource.AddMediaSource(mediaItem.ToMediaSource());
            }
            _player.Prepare(_mediaSource);
            _player.SeekTo(windowIndex, 0);
        }

        public void OnPrepareFromSearch(string p0, bool p1, Bundle p2)
        {
            _mediaSource.Clear();
            foreach (var mediaItem in MediaManager.Queue.Where(x => x.Title == p0))
            {
                _mediaSource.AddMediaSource(mediaItem.ToMediaSource());
            }
            _player.Prepare(_mediaSource);
        }

        public void OnPrepareFromUri(global::Android.Net.Uri p0, bool p1, Bundle p2)
        {
            _mediaSource.Clear();
            var windowIndex = 0;
            foreach (var mediaItem in MediaManager.Queue)
            {
                var uri = global::Android.Net.Uri.Parse(mediaItem.MediaUri);
                if (uri.Equals(p0))
                    windowIndex = MediaManager.Queue.IndexOf(mediaItem);

                _mediaSource.AddMediaSource(mediaItem.ToMediaSource());
            }
            _player.Prepare(_mediaSource);
            _player.SeekTo(windowIndex, 0);
        }
    }
}
