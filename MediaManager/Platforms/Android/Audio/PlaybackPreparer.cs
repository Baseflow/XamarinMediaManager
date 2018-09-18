using System;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Upstream;
using System.Linq;

namespace MediaManager.Platforms.Android.Audio
{
    public class PlaybackPreparer : Java.Lang.Object, MediaSessionConnector.IPlaybackPreparer
    {
        private IExoPlayer _player;
        private DefaultDataSourceFactory _defaultDataSourceFactory;
        private ConcatenatingMediaSource _mediaSource;
        private IMediaManager mediaManager = CrossMediaManager.Current;

        public PlaybackPreparer(IExoPlayer player, DefaultDataSourceFactory dataSourceFactory, ConcatenatingMediaSource mediaSource)
        {
            _player = player;
            _defaultDataSourceFactory = dataSourceFactory;
            _mediaSource = mediaSource;
        }

        public PlaybackPreparer(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
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
            foreach (var mediaItem in mediaManager.MediaQueue)
            {
                var uri = global::Android.Net.Uri.Parse(mediaItem.MediaUri);
                var extractorMediaSource = new ExtractorMediaSource.Factory(_defaultDataSourceFactory)
                    .SetTag(mediaItem.ToMediaDescription())
                    .CreateMediaSource(uri);
                _mediaSource.AddMediaSource(extractorMediaSource);
            }
            _player.Prepare(_mediaSource);

            //Only in case of Prepare set PlayWhenReady to true because we use this to load in the whole queue
            _player.PlayWhenReady = true;
        }

        public void OnPrepareFromMediaId(string mediaId, Bundle p1)
        {
            _mediaSource.Clear();
            int windowIndex = 0;
            foreach (var mediaItem in mediaManager.MediaQueue)
            {
                var uri = global::Android.Net.Uri.Parse(mediaItem.MediaUri);
                if (mediaItem.MediaId == mediaId)
                    windowIndex = mediaManager.MediaQueue.IndexOf(mediaItem);

                var extractorMediaSource = new ExtractorMediaSource.Factory(_defaultDataSourceFactory)
                    .SetTag(mediaItem.ToMediaDescription())
                    .CreateMediaSource(uri);
                _mediaSource.AddMediaSource(extractorMediaSource);
            }
            _player.Prepare(_mediaSource);
            _player.SeekTo(windowIndex, 0);
        }

        public void OnPrepareFromSearch(string searchTerm, Bundle p1)
        {
            _mediaSource.Clear();
            foreach (var mediaItem in mediaManager.MediaQueue.Where(x => x.Title == searchTerm))
            {
                var uri = global::Android.Net.Uri.Parse(mediaItem.MediaUri);
                var extractorMediaSource = new ExtractorMediaSource.Factory(_defaultDataSourceFactory)
                    .SetTag(mediaItem.ToMediaDescription())
                    .CreateMediaSource(uri);
                _mediaSource.AddMediaSource(extractorMediaSource);
            }
            _player.Prepare(_mediaSource);
        }

        public void OnPrepareFromUri(global::Android.Net.Uri mediaUri, Bundle p1)
        {
            _mediaSource.Clear();
            int windowIndex = 0;
            foreach (var mediaItem in mediaManager.MediaQueue)
            {
                var uri = global::Android.Net.Uri.Parse(mediaItem.MediaUri);
                if (uri.Equals(mediaUri))
                    windowIndex = mediaManager.MediaQueue.IndexOf(mediaItem);

                var extractorMediaSource = new ExtractorMediaSource.Factory(_defaultDataSourceFactory)
                    .SetTag(mediaItem.ToMediaDescription())
                    .CreateMediaSource(uri);
                _mediaSource.AddMediaSource(extractorMediaSource);
            }
            _player.Prepare(_mediaSource);
            _player.SeekTo(windowIndex, 0);
        }
    }
}
