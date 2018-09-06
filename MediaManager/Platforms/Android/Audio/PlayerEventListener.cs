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
        public PlayerEventListener()
        {
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
    }
}
