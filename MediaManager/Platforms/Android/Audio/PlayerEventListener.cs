using System;
using System.Collections.Generic;
using System.Text;
using Android.Runtime;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Metadata;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;

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

        public override void OnPlayerStateChanged(bool playWhenReady, int playbackState)
        {
            if (playWhenReady)
            {
                switch (playbackState)
                {
                    case Player.StateBuffering:
                        player.BufferedTimer.Start();
                        player.StatusTimer.Start();
                        break;
                    case Player.StateReady:
                        player.StatusTimer.Start();
                        break;
                    case Player.StateEnded:
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
    }
}
