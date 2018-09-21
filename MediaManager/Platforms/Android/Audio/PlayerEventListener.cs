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
        AudioPlayer player;
        public PlayerEventListener(AudioPlayer player)
        {
            this.player = player;
        }

        public PlayerEventListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

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
                    player.OnMediaItemFinished();
                    break;
                case Player.DiscontinuityReasonInternal:
                    break;
            }
            base.OnPositionDiscontinuity(reason);
        }

        public override void OnPlayerStateChanged(bool playWhenReady, int playbackState)
        {
            if (playWhenReady)
            {
                switch (playbackState)
                {
                    case Player.StateBuffering:
                    case Player.StateReady:
                        player.BufferedTimer.Start();
                        player.StatusTimer.Start();
                        player.OnMediaItemChanged();
                        break;
                    case Player.StateEnded:
                        player.OnMediaItemFinished();
                        player.BufferedTimer.Stop();
                        player.StatusTimer.Stop();
                        break;
                    case Player.StateIdle:
                        player.BufferedTimer.Stop();
                        player.StatusTimer.Stop();
                        break;
                }
            }
            else
            {
                player.BufferedTimer.Stop();
                player.StatusTimer.Stop();
            }

            base.OnPlayerStateChanged(playWhenReady, playbackState);
        }

        public override void OnPlayerError(ExoPlaybackException error)
        {
            //CrossMediaManager.Current.OnMediaItemFailed(this, new MediaItemFailedEventArgs(CrossMediaManager.Current.MediaQueue[player.Player.CurrentWindowIndex], error.InnerException, error.Message));
            base.OnPlayerError(error);
        }

        public override void OnLoadingChanged(bool isLoading)
        {
            base.OnLoadingChanged(isLoading);
        }

        public override void OnPlaybackParametersChanged(PlaybackParameters playbackParameters)
        {
            base.OnPlaybackParametersChanged(playbackParameters);
        }

        public override void OnRepeatModeChanged(int repeatMode)
        {
            base.OnRepeatModeChanged(repeatMode);
        }

        public override void OnSeekProcessed()
        {
            base.OnSeekProcessed();
        }

        public override void OnShuffleModeEnabledChanged(bool shuffleModeEnabled)
        {
            base.OnShuffleModeEnabledChanged(shuffleModeEnabled);
        }

        public override void OnTimelineChanged(Timeline timeline, Java.Lang.Object manifest)
        {
            base.OnTimelineChanged(timeline, manifest);
        }

        public override void OnTimelineChanged(Timeline timeline, Java.Lang.Object manifest, int reason)
        {
            base.OnTimelineChanged(timeline, manifest, reason);
        }
    }
}
