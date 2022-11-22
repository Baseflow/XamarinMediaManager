using Android.Icu.Text;
using Android.Runtime;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Metadata;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.Video;
using Kotlin;
//using static Com.Google.Android.Exoplayer2.IPlayer;

namespace MediaManager.Platforms.Android.Player
{
    public class PlayerEventListener : Java.Lang.Object, Com.Google.Android.Exoplayer2.IPlayer.IListener // IEventListener
    {
        public PlayerEventListener()
        {
        }

        protected PlayerEventListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public Action<bool> OnLoadingChangedImpl { get; set; }
        public Action<int> OnRepeatModeChangedImpl { get; set; }
        public Action OnSeekProcessedImpl { get; set; }
        public Action<bool, int> OnPlayerStateChangedImpl { get; set; }
        public Action<bool> OnShuffleModeEnabledChangedImpl { get; set; }
        public Action<PlaybackParameters> OnPlaybackParametersChangedImpl { get; set; }
        public Action<int> OnPositionDiscontinuityIntImpl { get; set; }
        public Action<IPlayer.PositionInfo, IPlayer.PositionInfo, int> OnPositionDiscontinuityImpl { get; set; }
        public Action<ExoPlaybackException> OnPlayerErrorImpl { get; set; }
        public Action<Com.Google.Android.Exoplayer2.Tracks> OnTracksChangedImpl { get; set; }
        public Action<Timeline, int> OnTimelineChangedImpl { get; set; }
        public Action<bool> OnIsPlayingChangedImpl { get; set; }
        public Action<MediaItem, int> OnMediaItemTransitionImpl { get; set; }
        public Action<int> OnPlaybackStateChangedImpl { get; set; }
        public Action<bool, int> OnPlayWhenReadyChangedImpl { get; set; }
        public Action<IPlayer, IPlayer.Events> OnEventsImpl { get; set; }
        public Action<bool> OnIsLoadingChangedImpl { get; set; }
        public Action<System.Collections.Generic.IList<Metadata>> OnStaticMetadataChangedImpl { get; set; }
        public Action<MediaMetadata> OnMediaMetadataChangedImpl { get; set; }
        public Action<Metadata> OnMetadataImpl { get; set; }
        public Action<int, bool> OnDeviceVolumeChangedImpl { get; set; }
        public Action<global::Com.Google.Android.Exoplayer2.IPlayer.Commands?> OnAvailableCommandsChangedImpl { get; set; }
        public Action<int, int> OnSurfaceSizeChangedImpl { get; set; }
        public Action<VideoSize> OnVideoSizeChangedImpl { get; set; }
        public Action OnRenderedFirstFrameImpl { get; set; }

        public void OnLoadingChanged(bool isLoading)
        {
            OnLoadingChangedImpl?.Invoke(isLoading);
        }

        public void OnRepeatModeChanged(int repeatMode)
        {
            OnRepeatModeChangedImpl?.Invoke(repeatMode);
        }

        public void OnSeekProcessed()
        {
            OnSeekProcessedImpl?.Invoke();
        }

        public void OnPlayerStateChanged(bool playWhenReady, int playbackState)
        {
            OnPlayerStateChangedImpl?.Invoke(playWhenReady, playbackState);
        }

        public void OnShuffleModeEnabledChanged(bool p0)
        {
            OnShuffleModeEnabledChangedImpl?.Invoke(p0);
        }

        public void OnPlaybackParametersChanged(PlaybackParameters playbackParameters)
        {
            OnPlaybackParametersChangedImpl?.Invoke(playbackParameters);
        }

        public void OnPositionDiscontinuity(int reason)
        {
            OnPositionDiscontinuityIntImpl?.Invoke(reason);
        }

        public void OnPositionDiscontinuity(IPlayer.PositionInfo oldPosition, IPlayer.PositionInfo newPosition, int reason)
        {
            OnPositionDiscontinuityImpl?.Invoke(oldPosition, newPosition, reason);
        }

        public void OnPlayerError(ExoPlaybackException e)
        {
            OnPlayerErrorImpl?.Invoke(e);
        }

        public void OnTracksChanged(Com.Google.Android.Exoplayer2.Tracks tracks)
        {
            OnTracksChangedImpl?.Invoke(tracks);
        }

        public void OnTimelineChanged(Timeline timeline, int time)
        {
            OnTimelineChangedImpl?.Invoke(timeline, time);
        }

        public void OnIsPlayingChanged(bool isPlaying)
        {
            OnIsPlayingChangedImpl?.Invoke(isPlaying);
        }

        public void OnMediaItemTransition(MediaItem MediaItem, int reason)
        {
            OnMediaItemTransitionImpl?.Invoke(MediaItem, reason);
        }

        public void OnPlaybackStateChanged(int state)
        {
            OnPlaybackStateChangedImpl?.Invoke(state);
        }

        public void OnPlayWhenReadyChanged(bool playWhenReady, int reason)
        {
            OnPlayWhenReadyChangedImpl?.Invoke(playWhenReady, reason);
        }

        public void OnEvents(IPlayer Player, IPlayer.Events Events)
        {
            OnEventsImpl?.Invoke(Player, Events);
        }

        public void OnIsLoadingChanged(bool changed)
        {
            OnIsLoadingChangedImpl?.Invoke(changed);
        }

        public void OnStaticMetadataChanged(System.Collections.Generic.IList<Metadata> metadataList)
        {
            OnStaticMetadataChangedImpl?.Invoke(metadataList);
        }

        public void OnMediaMetadataChanged(MediaMetadata mediaMetadata)
        {
            OnMediaMetadataChangedImpl?.Invoke(mediaMetadata);
        }

        public void OnMetadata(Metadata metadata)
        {
            OnMetadataImpl?.Invoke(metadata);
        }

        public void OnDeviceVolumeChanged(int Volume, bool Muted)
        {
            OnDeviceVolumeChangedImpl?.Invoke(Volume, Muted);
        }

        public void OnAvailableCommandsChanged(global::Com.Google.Android.Exoplayer2.IPlayer.Commands? availableCommands)
        {
            OnAvailableCommandsChangedImpl?.Invoke(availableCommands);
        }

        //Video related methods
        public void OnSurfaceSizeChanged(int width, int hight)
        {
            OnSurfaceSizeChangedImpl?.Invoke(width, hight);
        }

        public void OnVideoSizeChanged(VideoSize size)
        {
            OnVideoSizeChangedImpl?.Invoke(size);
        }

        public void OnRenderedFirstFrame()
        {
            OnRenderedFirstFrameImpl?.Invoke();
        }
    }
}
