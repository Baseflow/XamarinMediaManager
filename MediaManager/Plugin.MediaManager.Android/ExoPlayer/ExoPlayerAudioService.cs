using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Metadata;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Source.Dash;
using Com.Google.Android.Exoplayer2.Source.Hls;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using Java.IO;
using Java.Lang;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;
using Console = System.Console;
using Object = Java.Lang.Object;

namespace Plugin.MediaManager.ExoPlayer
{
    [Service]
    [IntentFilter(new[] { ActionPlay, ActionPause, ActionStop, ActionTogglePlayback, ActionNext, ActionPrevious })]
    public class ExoPlayerAudioService : MediaServiceBase,
        IExoPlayerEventListener,
        TrackSelector.IEventListener
    {
        private SimpleExoPlayer _mediaPlayer;
        
        private CancellationTokenSource _onBufferingCancellationSource = new CancellationTokenSource();

        public override TimeSpan Position => TimeSpan.FromMilliseconds(Convert.ToInt32(_mediaPlayer?.CurrentPosition));
        public override TimeSpan Duration => TimeSpan.FromMilliseconds(Convert.ToInt32(_mediaPlayer?.Duration));
        public override TimeSpan Buffered => TimeSpan.FromMilliseconds(Convert.ToInt32(_mediaPlayer?.BufferedPosition));

        public override void InitializePlayer()
        {
            var mainHandler = new Handler();
            var trackSelector = new DefaultTrackSelector(mainHandler);
            trackSelector.AddListener(this);
            var loadControl = new DefaultLoadControl();
            if (_mediaPlayer == null)
            {
                _mediaPlayer = ExoPlayerFactory.NewSimpleInstance(ApplicationContext, trackSelector, loadControl);
                _mediaPlayer.AddListener(this);
            }

            StatusChanged += (sender, args) => OnStatusChangedHandler(args);
        }

        public override void InitializePlayerWithUrl(string audioUrl)
        {
            throw new NotImplementedException();
        }

        public override void SetMediaPlayerOptions()
        {
        }

        public override async Task Play(IMediaFile mediaFile = null)
        {
            await base.Play(mediaFile);
            _mediaPlayer.PlayWhenReady = true;
            ManuallyPaused = false;
        }

        public override Task Seek(TimeSpan position)
        {
            return Task.Run(() =>
            {
                _mediaPlayer?.SeekTo(Convert.ToInt64(position.TotalMilliseconds));
            });
        }

        public override async Task Pause()
        {
            _mediaPlayer.PlayWhenReady = false;
            ManuallyPaused = true;
            await base.Pause();
        }

        public override Task Play(IEnumerable<IMediaFile> mediaFiles)
        {
            throw new NotImplementedException();
        }

        public override Task TogglePlayPause(bool forceToPlay)
        {
            if(_mediaPlayer != null)
                _mediaPlayer.PlayWhenReady = !_mediaPlayer.PlayWhenReady || forceToPlay;
            return Task.CompletedTask;
        }

        public override void SetVolume(float leftVolume, float rightVolume)
        {
            _mediaPlayer.Volume = leftVolume;
        }

        public override async Task<bool> SetMediaPlayerDataSource()
        {
            var source = GetSourceByUrl(CurrentFile.Url);
            _mediaPlayer.Prepare(source);
            return await Task.FromResult(true);
        }

        #region ************ ExoPlayer Events *****************

        public void OnLoadingChanged(bool isLoading)
        {
            Console.WriteLine("Loading changed");
        }

        public void OnPlayerError(ExoPlaybackException ex)
        {
            OnMediaFailed(new MediaFailedEventArgs(ex.Message, ex));
        }

        public void OnPlayerStateChanged(bool playWhenReady, int state)
        {
            if (state == Com.Google.Android.Exoplayer2.ExoPlayer.StateEnded)
            {
                OnMediaFinished(new MediaFinishedEventArgs(CurrentFile));
            }
            else
            {
                var status = GetStatusByIntValue(state);
                var compatState = GetCompatValueByStatus(status);
                OnStatusChanged(new StatusChangedEventArgs(status));
                SessionManager.UpdatePlaybackState(compatState, Position.Seconds);
            }
        }

        public void OnPositionDiscontinuity()
        {
            Console.WriteLine("OnPositionDiscontinuity");
        }

        public void OnTimelineChanged(Timeline timeline, Object manifest)
        {
            Console.WriteLine("OnTimelineChanged");
        }

        public void OnTrackSelectionsChanged(TrackSelections p0)
        {
            Console.WriteLine("TrackSelectionChanged");
        }

        /* TODO: Implement IOutput Interface => https://github.com/martijn00/ExoPlayerXamarin/issues/38
         */
        //public void OnMetadata(Object obj)
        //{
        //    Console.WriteLine("OnMetadata");
        //}
        #endregion

        private MediaPlayerStatus GetStatusByIntValue(int state)
        {
            switch (state)
            {
                case Com.Google.Android.Exoplayer2.ExoPlayer.StateBuffering:
                    return MediaPlayerStatus.Buffering;
                case Com.Google.Android.Exoplayer2.ExoPlayer.StateReady:
                    return !ManuallyPaused && !TransientPaused ? MediaPlayerStatus.Playing : MediaPlayerStatus.Paused;
                case Com.Google.Android.Exoplayer2.ExoPlayer.StateIdle:
                    return MediaPlayerStatus.Loading;
                default:
                    return MediaPlayerStatus.Failed;
            }
        }

        private int GetCompatValueByStatus(MediaPlayerStatus state)
        {
            switch (state)
            {
                case MediaPlayerStatus.Buffering:
                    return PlaybackStateCompat.StateBuffering;
                case MediaPlayerStatus.Failed:
                    return PlaybackStateCompat.StateError;
                case MediaPlayerStatus.Loading:
                    return PlaybackStateCompat.StateConnecting;
                case MediaPlayerStatus.Paused:
                    return PlaybackStateCompat.StatePaused;
                case MediaPlayerStatus.Playing:
                    return PlaybackStateCompat.StatePlaying;
                case MediaPlayerStatus.Stopped:
                    return PlaybackStateCompat.StateStopped;
                default:
                    return PlaybackStateCompat.StateNone;
            }
        }

        [Obsolete("deprecated")]
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            HandleIntent(intent);
            return base.OnStartCommand(intent, flags, startId);
        }

        private void OnStatusChangedHandler(StatusChangedEventArgs args)
        {
            if (args.Status == MediaPlayerStatus.Buffering)
            {
                CancelBufferingTask();
                 Task.Run(() => OnBuffering(), _onBufferingCancellationSource.Token);
            }
            if (args.Status == MediaPlayerStatus.Stopped || args.Status == MediaPlayerStatus.Failed)
                CancelBufferingTask();
        }

        private void CancelBufferingTask()
        {
            if (!_onBufferingCancellationSource.Token.CanBeCanceled) return;
            _onBufferingCancellationSource.Cancel();
            _onBufferingCancellationSource.Dispose();
            _onBufferingCancellationSource = new CancellationTokenSource();
        }

        private void OnBuffering()
        {
            if(_mediaPlayer == null) return;

            if (MediaPlayerState != PlaybackStateCompat.StateStopped &&
                MediaPlayerState != PlaybackStateCompat.StateError && MediaPlayerState != PlaybackStateCompat.StateNone)
            {
                OnBufferingChanged(new BufferingChangedEventArgs(_mediaPlayer.BufferedPercentage, TimeSpan.FromMilliseconds(Convert.ToInt64(_mediaPlayer.BufferedPosition))));
                if (_mediaPlayer.BufferedPercentage < 100)
                    Task.Delay(1000).ContinueWith(task => OnBuffering(), _onBufferingCancellationSource.Token);
            }
            else
                OnBufferingChanged(new BufferingChangedEventArgs(0, TimeSpan.Zero));
        }

        private IMediaSource GetSourceByUrl(string url)
        {
            var bandwithMeter = new DefaultBandwidthMeter();
            var httpFactory = new DefaultHttpDataSourceFactory(ExoPlayerUtil.GetUserAgent(this, ApplicationInfo.Name), bandwithMeter);
            var factory = new HttpSourceFactory(httpFactory, RequestProperties);
            var extractorFactory = new DefaultExtractorsFactory();
            var uri = Android.Net.Uri.Parse(url);
            return new ExtractorMediaSource(uri
                , factory
                , extractorFactory, null, null);
        }
    }

    public class HttpSourceFactory : Object, IDataSourceFactory
    {
        private DefaultHttpDataSourceFactory _httpFactory;
        private Dictionary<string, string> _headers;
        public HttpSourceFactory(DefaultHttpDataSourceFactory httpFactory, Dictionary<string, string> headers)
        {
            _httpFactory = httpFactory;
            _headers = headers;
        }

        public HttpSourceFactory()
        {
            Console.WriteLine("HSF called");
        }

        public IDataSource CreateDataSource()
        {
            var source = _httpFactory.CreateDataSource() as DefaultHttpDataSource;
            if (!_headers.Any())
                return source;

            foreach (var header in _headers)
            {
                source?.SetRequestProperty(header.Key, header.Value);
            }
            return source;
        }

        public IDataSource createDataSource()
        {
            return CreateDataSource();
        }
    }

}
