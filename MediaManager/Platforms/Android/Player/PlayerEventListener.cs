using Android.Icu.Text;
using Android.Runtime;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Audio;
using Com.Google.Android.Exoplayer2.Metadata;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Text;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.Video;

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

        public Action<AudioAttributes> OnAudioAttributesChangedImpl { get; set; }
        public Action<int> OnAudioSessionIdChangedImpl { get; set; }
        public Action<IPlayer.Commands> OnAvailableCommandsChangedImpl { get; set; }
        public Action<CueGroup> OnCuesImpl { get; set; }
        public Action<DeviceInfo> OnDeviceInfoChangedImpl { get; set; }
        public Action<int, bool> OnDeviceVolumeChangedImpl { get; set; }
        public Action<IPlayer, IPlayer.Events> OnEventsImpl { get; set; }
        public Action<bool> OnIsLoadingChangedImpl { get; set; }
        public Action<bool> OnIsPlayingChangedImpl { get; set; }
        public Action<bool> OnLoadingChangedImpl { get; set; }
        public Action<long> OnMaxSeekToPreviousPositionChangedImpl { get; set; }
        public Action<MediaItem, int> OnMediaItemTransitionImpl { get; set; }
        public Action<MediaMetadata> OnMediaMetadataChangedImpl { get; set; }
        public Action<Metadata> OnMetadataImpl { get; set; }
        public Action<PlaybackParameters> OnPlaybackParametersChangedImpl { get; set; }
        public Action<int> OnPlaybackStateChangedImpl { get; set; }
        public Action<int> OnPlaybackSuppressionReasonChangedImpl { get; set; }
        public Action<PlaybackException> OnPlayerErrorImpl { get; set; }
        public Action<PlaybackException> OnPlayerErrorChangedImpl { get; set; }
        public Action<bool, int> OnPlayerStateChangedImpl { get; set; }
        public Action<MediaMetadata> OnPlaylistMetadataChangedImpl { get; set; }
        public Action<bool, int> OnPlayWhenReadyChangedImpl { get; set; }
        public Action<int> OnPositionDiscontinuityImpl { get; set; }
        public Action OnRenderedFirstFrameImpl { get; set; }
        public Action<int> OnRepeatModeChangedImpl { get; set; }
        public Action<long> OnSeekBackIncrementChangedImpl { get; set; }
        public Action<long> OnSeekForwardIncrementChangedImpl { get; set; }
        public Action OnSeekProcessedImpl { get; set; }
        public Action<bool> OnShuffleModeEnabledChangedImpl { get; set; }
        public Action<bool> OnSkipSilenceEnabledChangedImpl { get; set; }
        public Action<int, int> OnSurfaceSizeChangedImpl { get; set; }
        public Action<Timeline, int> OnTimelineChangedImpl { get; set; }
        public Action<Tracks> OnTracksChangedImpl { get; set; }
        public Action<TrackSelectionParameters> OnTrackSelectionParametersChangedImpl { get; set; }
        public Action<VideoSize> OnVideoSizeChangedImpl { get; set; }
        public Action<float> OnVolumeChangedImpl { get; set; }


        public void OnAudioAttributesChanged(AudioAttributes audioAttributes)
        {
            OnAudioAttributesChangedImpl?.Invoke(audioAttributes);
        }

        public void OnAudioSessionIdChanged(int audioSessionId)
        {
            OnAudioSessionIdChangedImpl?.Invoke(audioSessionId);
        }

        public void OnAvailableCommandsChanged(IPlayer.Commands availableCommands)
        {
            OnAvailableCommandsChangedImpl?.Invoke(availableCommands);
        }

        public void OnCues(CueGroup cueGroup)
        {
            OnCuesImpl?.Invoke(cueGroup);
        }

        public void OnDeviceInfoChanged(DeviceInfo deviceInfo)
        {
            OnDeviceInfoChangedImpl?.Invoke(deviceInfo);
        }

        public void OnDeviceVolumeChanged(int volume, bool muted)
        {
            OnDeviceVolumeChangedImpl?.Invoke(volume, muted);
        }

        public void OnEvents(IPlayer player, IPlayer.Events events)
        {
            OnEventsImpl?.Invoke(player, events);
        }

        public void OnIsLoadingChanged(bool isLoading)
        {
            OnLoadingChangedImpl?.Invoke(isLoading);
        }

        public void OnIsPlayingChanged(bool isPlaying)
        {
            OnIsPlayingChangedImpl?.Invoke(isPlaying);
        }

        public void OnLoadingChanged(bool isLoading)
        {
            OnLoadingChangedImpl?.Invoke(isLoading);
        }

        public void OnMaxSeekToPreviousPositionChanged(long maxSeekToPreviousPositionMs)
        {
            OnMaxSeekToPreviousPositionChangedImpl?.Invoke(maxSeekToPreviousPositionMs);
        }

        public void OnMediaItemTransition(MediaItem mediaItem, int reason)
        {
            OnMediaItemTransitionImpl?.Invoke(mediaItem, reason);
        }

        public void OnMediaMetadataChanged(MediaMetadata mediaMetadata)
        {
            OnMediaMetadataChangedImpl?.Invoke(mediaMetadata);
        }

        public void OnMetadata(Metadata metadata)
        {
            OnMetadataImpl?.Invoke(metadata);
        }

        public void OnPlaybackParametersChanged(PlaybackParameters playbackParameters)
        {
            OnPlaybackParametersChangedImpl?.Invoke(playbackParameters);
        }

        public void OnPlaybackStateChanged(int playbackState)
        {
            OnPlaybackStateChangedImpl?.Invoke(playbackState);
        }

        public void OnPlaybackSuppressionReasonChanged(int playbackSuppressionReason)
        {
            OnPlaybackSuppressionReasonChangedImpl?.Invoke(playbackSuppressionReason);
        }

        public void OnPlayerError(PlaybackException error)
        {
            OnPlayerErrorImpl?.Invoke(error);
        }

        public void OnPlayerErrorChanged(PlaybackException error)
        {
            OnPlayerErrorChangedImpl?.Invoke(error);
        }

        public void OnPlayerStateChanged(bool playWhenReady, int playbackState)
        {
            OnPlayerStateChangedImpl?.Invoke(playWhenReady, playbackState);
        }

        public void OnPlaylistMetadataChanged(MediaMetadata mediaMetadata)
        {
            OnPlaylistMetadataChangedImpl?.Invoke(mediaMetadata);
        }

        public void OnPlayWhenReadyChanged(bool playWhenReady, int reason)
        {
            OnPlayWhenReadyChangedImpl?.Invoke(playWhenReady, reason);
        }

        public void OnPositionDiscontinuity(int reason)
        {
            OnPositionDiscontinuityImpl?.Invoke(reason);
        }

        public void OnRenderedFirstFrame()
        {
            OnRenderedFirstFrameImpl?.Invoke();
        }

        public void OnRepeatModeChanged(int repeatMode)
        {
            OnRepeatModeChangedImpl?.Invoke(repeatMode);
        }

        public void OnSeekBackIncrementChanged(long seekBackIncrementMs)
        {
            OnSeekBackIncrementChangedImpl?.Invoke(seekBackIncrementMs);
        }

        public void OnSeekForwardIncrementChanged(long seekForwardIncrementMs)
        {
            OnSeekForwardIncrementChangedImpl?.Invoke(seekForwardIncrementMs);
        }

        public void OnSeekProcessed()
        {
            OnSeekProcessedImpl?.Invoke();
        }

        public void OnShuffleModeEnabledChanged(bool shuffleModeEnabled)
        {
            OnShuffleModeEnabledChangedImpl?.Invoke(shuffleModeEnabled);
        }

        public void OnSkipSilenceEnabledChanged(bool skipSilenceEnabled)
        {
            OnSkipSilenceEnabledChangedImpl?.Invoke(skipSilenceEnabled);
        }

        public void OnSurfaceSizeChanged(int width, int height)
        {
            OnSurfaceSizeChangedImpl?.Invoke(width, height);
        }

        public void OnTimelineChanged(Timeline timeline, int reason)
        {
            OnTimelineChangedImpl?.Invoke(timeline, reason);
        }

        public void OnTracksChanged(Tracks tracks)
        {
            OnTracksChangedImpl?.Invoke(tracks);
        }

        public void OnTrackSelectionParametersChanged(TrackSelectionParameters parameters)
        {
            OnTrackSelectionParametersChangedImpl?.Invoke(parameters);
        }

        public void OnVideoSizeChanged(VideoSize videoSize)
        {
            OnVideoSizeChangedImpl?.Invoke(videoSize);
        }

        public void OnVolumeChanged(float volume)
        {
            OnVolumeChangedImpl?.Invoke(volume);
        }
        /*
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
}*/
    }
}
