using System;
using System.Collections.Generic;
using System.Text;
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
            _player.Prepare(_mediaSource);
        }

        public void OnPrepareFromMediaId(string mediaId, Bundle p1)
        {
            _mediaSource.Clear();
            foreach (var item in mediaManager.MediaQueue.Where(x => x.MetadataMediaId == mediaId))
            {
                var uri = global::Android.Net.Uri.Parse(item.MetadataMediaUri);
                var extractorMediaSource = new ExtractorMediaSource.Factory(_defaultDataSourceFactory).SetTag(item.GetMediaDescription()).CreateMediaSource(uri);
                _mediaSource.AddMediaSource(extractorMediaSource);
            }
            _player.Prepare(_mediaSource);
        }

        public void OnPrepareFromSearch(string searchTerm, Bundle p1)
        {
            _mediaSource.Clear();
            foreach (var item in mediaManager.MediaQueue.Where(x => x.MetadataTitle == searchTerm))
            {
                var uri = global::Android.Net.Uri.Parse(item.MetadataMediaUri);
                var extractorMediaSource = new ExtractorMediaSource.Factory(_defaultDataSourceFactory).SetTag(item.GetMediaDescription()).CreateMediaSource(uri);
                _mediaSource.AddMediaSource(extractorMediaSource);
            }
            _player.Prepare(_mediaSource);
        }

        public void OnPrepareFromUri(global::Android.Net.Uri mediaUri, Bundle p1)
        {
            _mediaSource.Clear();
            foreach (var item in mediaManager.MediaQueue)
            {
                var uri = global::Android.Net.Uri.Parse(item.MetadataMediaUri);
                var extractorMediaSource = new ExtractorMediaSource.Factory(_defaultDataSourceFactory).SetTag(item.GetMediaDescription()).CreateMediaSource(uri);
                _mediaSource.AddMediaSource(extractorMediaSource);
            }
            _player.Prepare(_mediaSource);
        }
    }
}
