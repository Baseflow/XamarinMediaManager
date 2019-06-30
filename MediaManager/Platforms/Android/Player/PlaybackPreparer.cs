using System;
using System.Linq;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Source;
using Java.Lang;

namespace MediaManager.Platforms.Android.Media
{
    public class MediaSessionConnectorPlaybackPreparer : Java.Lang.Object, MediaSessionConnector.IPlaybackPreparer
    {
        protected IExoPlayer _player;
        protected ConcatenatingMediaSource _mediaSource;
        protected IMediaManager MediaManager = CrossMediaManager.Android;

        public MediaSessionConnectorPlaybackPreparer(IExoPlayer player, ConcatenatingMediaSource mediaSource)
        {
            _player = player;
            _mediaSource = mediaSource;
        }

        protected MediaSessionConnectorPlaybackPreparer(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public long SupportedPrepareActions =>
                PlaybackStateCompat.ActionPrepare |
                PlaybackStateCompat.ActionPrepareFromMediaId |
                PlaybackStateCompat.ActionPrepareFromSearch |
                PlaybackStateCompat.ActionPrepareFromUri |
                PlaybackStateCompat.ActionPlayFromMediaId |
                PlaybackStateCompat.ActionPlayFromSearch |
                PlaybackStateCompat.ActionPlayFromUri;

        public string[] GetCommands()
        {
            return null;
        }

        public void OnCommand(IPlayer p0, string p1, Bundle p2, ResultReceiver p3)
        {
        }

        public void OnPrepare()
        {
            _mediaSource.Clear();

            //TODO: Thread.Sleep hack to get it working. Seems fixed in 2.9.6 https://github.com/google/ExoPlayer/issues/5464
            Thread.Sleep(200);

            var mediaItems = MediaManager.MediaQueue.Select(x => x.ToMediaSource()).ToList();
            _mediaSource.AddMediaSources(mediaItems);

            _player.Prepare(_mediaSource);

            //Only in case of Prepare set PlayWhenReady to true because we use this to load in the whole queue
            _player.PlayWhenReady = true;
        }

        public void OnPrepareFromMediaId(string mediaId, Bundle p1)
        {
            _mediaSource.Clear();
            int windowIndex = 0;
            foreach (var mediaItem in MediaManager.MediaQueue)
            {
                if (mediaItem.MediaId == mediaId)
                    windowIndex = MediaManager.MediaQueue.IndexOf(mediaItem);

                _mediaSource.AddMediaSource(mediaItem.ToMediaSource());
            }
            _player.Prepare(_mediaSource);
            _player.SeekTo(windowIndex, 0);
        }

        public void OnPrepareFromSearch(string searchTerm, Bundle p1)
        {
            _mediaSource.Clear();
            foreach (var mediaItem in MediaManager.MediaQueue.Where(x => x.Title == searchTerm))
            {
                _mediaSource.AddMediaSource(mediaItem.ToMediaSource());
            }
            _player.Prepare(_mediaSource);
        }

        public void OnPrepareFromUri(global::Android.Net.Uri mediaUri, Bundle p1)
        {
            _mediaSource.Clear();
            int windowIndex = 0;
            foreach (var mediaItem in MediaManager.MediaQueue)
            {
                var uri = global::Android.Net.Uri.Parse(mediaItem.MediaUri);
                if (uri.Equals(mediaUri))
                    windowIndex = MediaManager.MediaQueue.IndexOf(mediaItem);

                _mediaSource.AddMediaSource(mediaItem.ToMediaSource());
            }
            _player.Prepare(_mediaSource);
            _player.SeekTo(windowIndex, 0);
        }
    }
}
