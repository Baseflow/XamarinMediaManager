using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Provider;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;
using Object = Java.Lang.Object;

namespace Plugin.MediaManager.ExoPlayer
{
    public class ExoPlayerAudioService : MediaServiceBase,
        IExoPlayerEventListener,
        TrackSelector.IEventListener
    {
        private SimpleExoPlayer _mediaPlayer;

        public override TimeSpan Position => TimeSpan.FromMilliseconds(Convert.ToInt32(_mediaPlayer?.CurrentPosition));
        public override TimeSpan Duration => TimeSpan.FromMilliseconds(Convert.ToInt32(_mediaPlayer?.Duration));
        public override TimeSpan Buffered => TimeSpan.FromMilliseconds(Convert.ToInt32(_mediaPlayer?.BufferedPosition));

        public override void InitializePlayer()
        {
            var mainHandler = new Handler();
            var trackSelector = new DefaultTrackSelector(mainHandler);
            trackSelector.AddListener(this);
            var loadControl = new DefaultLoadControl();
            _mediaPlayer = ExoPlayerFactory.NewSimpleInstance(ApplicationContext, trackSelector, loadControl);
        }

        public override void InitializePlayerWithUrl(string audioUrl)
        {
            throw new NotImplementedException();
        }

        public override void SetMediaPlayerOptions()
        {
            _mediaPlayer.AddListener(this);
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

        public override Task Pause()
        {
            return Task.Run(() =>
            {
                _mediaPlayer.PlayWhenReady = false;
                ManuallyPaused = true;
            });
        }

        public override Task Play(IEnumerable<IMediaFile> mediaFiles)
        {
            throw new NotImplementedException();
        }

        public override Task TogglePlayPause(bool forceToPlay)
        {
            return Task.Run(() =>
            {
                if (_mediaPlayer.PlayWhenReady)
                    _mediaPlayer.PlayWhenReady = forceToPlay;
            });
        }

        public override void SetVolume(float leftVolume, float rightVolume)
        {
            _mediaPlayer.Volume = leftVolume;
        }

        public override async Task<bool> SetMediaPlayerDataSource()
        {
            var bandwithMeter = new DefaultBandwidthMeter();
            var dataSourceFactory = new DefaultDataSourceFactory(this, ExoPlayerUtil.GetUserAgent(this, ApplicationInfo.Name), bandwithMeter);
            var extractorFactory = new DefaultExtractorsFactory();
            var source = new ExtractorMediaSource(MediaStore.Audio.Media.GetContentUriForPath(CurrentFile.Url)
                , dataSourceFactory
                , extractorFactory, null, null);

            _mediaPlayer.Prepare(source);
            return await Task.FromResult(true);
        }

        #region ************ ExoPlayer Events *****************

        public void OnLoadingChanged(bool isLoading)
        {

        }

        public void OnPlayerError(ExoPlaybackException ex)
        {
            OnMediaFailed(new MediaFailedEventArgs(ex.Message, ex));
        }

        public void OnPlayerStateChanged(bool playWhenReady, int state)
        {
            var status = GetStatusByIntValue(state);
            OnStatusChanged(new StatusChangedEventArgs(status));
        }

        public void OnPositionDiscontinuity()
        {
            
        }

        public void OnTimelineChanged(Timeline timeline, Object manifest)
        {
            
        }

        public void OnTrackSelectionsChanged(TrackSelections p0)
        {

        }

        /* TODO: Implement IOutput Interface => https://github.com/martijn00/ExoPlayerXamarin/issues/38
         */
        public void OnMetadata(Object obj)
        {
            Console.WriteLine("OnMetadata");
        }
        

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
    }
}
