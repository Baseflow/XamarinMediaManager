using System;
using System.Collections.Generic;
using System.Text;
using Android.Runtime;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Metadata;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Java.Lang;
using MediaManager.Media;

namespace MediaManager.Platforms.Android.Audio
{
    public class PlayerEventListener : PlayerDefaultEventListener
    {
        public PlayerEventListener()
        {
        }

        public PlayerEventListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {

        }

        public Action<TrackGroupArray, TrackSelectionArray> OnTracksChangedImpl { get; set; }
        public Action<int> OnPositionDiscontinuityImpl { get; set; }
        public Action<bool, int> OnPlayerStateChangedImpl { get; set; }
        public Action<ExoPlaybackException> OnPlayerErrorImpl { get; set; }
        public Action<bool> OnLoadingChangedImpl { get; set; }
        public Action<PlaybackParameters> OnPlaybackParametersChangedImpl { get; set; }
        public Action<int> OnRepeatModeChangedImpl { get; set; }
        public Action OnSeekProcessedImpl { get; set; }
        public Action<bool> OnShuffleModeEnabledChangedImpl { get; set; }
        public Action<Timeline, Java.Lang.Object, int> OnTimelineChangedImpl { get; set; }

        public override void OnTracksChanged(TrackGroupArray trackGroups, TrackSelectionArray trackSelections)
        {
            for (int i = 0; i < trackGroups.Length; i++)
            {
                TrackGroup trackGroup = trackGroups.Get(i);
                for (int j = 0; j < trackGroup.Length; j++)
                {
                    Metadata trackMetadata = trackGroup.GetFormat(j).Metadata;
                    if (trackMetadata != null)
                    {
                        // We found metadata. Do something with it here!
                    }
                }
            }
            OnTracksChangedImpl?.Invoke(trackGroups, trackSelections);
            base.OnTracksChanged(trackGroups, trackSelections);
        }

        public override void OnPositionDiscontinuity(int reason)
        {
            switch (reason)
            {
                case Player.DiscontinuityReasonAdInsertion:
                case Player.DiscontinuityReasonSeek:
                case Player.DiscontinuityReasonSeekAdjustment:
                    break;
                case Player.DiscontinuityReasonPeriodTransition:
                    //player.OnMediaItemFinished();
                    break;
                case Player.DiscontinuityReasonInternal:
                    break;
            }
            OnPositionDiscontinuityImpl?.Invoke(reason);
            base.OnPositionDiscontinuity(reason);
        }

        public override void OnPlayerStateChanged(bool playWhenReady, int playbackState)
        {
            OnPlayerStateChangedImpl?.Invoke(playWhenReady, playbackState);
            base.OnPlayerStateChanged(playWhenReady, playbackState);
        }

        public override void OnPlayerError(ExoPlaybackException error)
        {
            //CrossMediaManager.Current.OnMediaItemFailed(this, new MediaItemFailedEventArgs(CrossMediaManager.Current.MediaQueue[player.Player.CurrentWindowIndex], error.InnerException, error.Message));
            OnPlayerErrorImpl?.Invoke(error);
            base.OnPlayerError(error);
        }

        public override void OnLoadingChanged(bool isLoading)
        {
            OnLoadingChangedImpl?.Invoke(isLoading);
            base.OnLoadingChanged(isLoading);
        }

        public override void OnPlaybackParametersChanged(PlaybackParameters playbackParameters)
        {
            OnPlaybackParametersChangedImpl?.Invoke(playbackParameters);
            base.OnPlaybackParametersChanged(playbackParameters);
        }

        public override void OnRepeatModeChanged(int repeatMode)
        {
            OnRepeatModeChangedImpl?.Invoke(repeatMode);
            base.OnRepeatModeChanged(repeatMode);
        }

        public override void OnSeekProcessed()
        {
            OnSeekProcessedImpl?.Invoke();
            base.OnSeekProcessed();
        }

        public override void OnShuffleModeEnabledChanged(bool shuffleModeEnabled)
        {
            OnShuffleModeEnabledChangedImpl?.Invoke(shuffleModeEnabled);
            base.OnShuffleModeEnabledChanged(shuffleModeEnabled);
        }

        public override void OnTimelineChanged(Timeline timeline, Java.Lang.Object manifest, int reason)
        {
            OnTimelineChangedImpl?.Invoke(timeline, manifest, reason);
            base.OnTimelineChanged(timeline, manifest, reason);
        }
    }
}
