using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using MediaManager.Audio;
using MediaManager.Media;

namespace MediaManager
{
    public class AudioPlayer : IAudioPlayer
    {
        private SimpleExoPlayer _player;

        public Dictionary<string, string> RequestHeaders { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MediaPlayerStatus Status => throw new NotImplementedException();

        public TimeSpan Position => throw new NotImplementedException();

        public TimeSpan Duration => throw new NotImplementedException();

        public TimeSpan Buffered => throw new NotImplementedException();

        private Context context { get; } = Application.Context;

        public Task Pause()
        {
            throw new NotImplementedException();
        }

        public Task Play(IMediaItem item)
        {
            var mediaUrl = item.MetadataMediaUri;
            var mediaUri = Android.Net.Uri.Parse(mediaUrl);

            var userAgent = Util.GetUserAgent(context, "ExoPlayerDemo");
            var defaultHttpDataSourceFactory = new DefaultHttpDataSourceFactory(userAgent);
            var defaultDataSourceFactory = new DefaultDataSourceFactory(context, null, defaultHttpDataSourceFactory);
            var extractorMediaSource = new ExtractorMediaSource(mediaUri, defaultDataSourceFactory, new DefaultExtractorsFactory(), null, null);
            var defaultBandwidthMeter = new DefaultBandwidthMeter();
            var adaptiveTrackSelectionFactory = new AdaptiveTrackSelection.Factory(defaultBandwidthMeter);
            var defaultTrackSelector = new DefaultTrackSelector(adaptiveTrackSelectionFactory);

            _player = ExoPlayerFactory.NewSimpleInstance(context, defaultTrackSelector);
            //MediaSessionConnector mediaSessionConnector = new MediaSessionConnector(mediaSession);
            //mediaSessionConnector.SetPlayer(_player, null, null);
            _player.Prepare(extractorMediaSource);
            _player.PlayWhenReady = true;

            return Task.CompletedTask;
        }

        public Task Play()
        {
            throw new NotImplementedException();
        }

        public Task Seek(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }
    }
}
