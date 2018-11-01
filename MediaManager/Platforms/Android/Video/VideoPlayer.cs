using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Media;
using Android.Runtime;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.UI;
using MediaManager.Media;
using MediaManager.Platforms.Android.Media;
using MediaManager.Playback;
using MediaManager.Video;

namespace MediaManager.Platforms.Android.Video
{
    /*public class VideoPlayer : Media.MediaPlayer
    {
        public VideoPlayer()
        {
        }

        public VideoPlayer(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        //TODO: Set the playerview
        public PlayerView playerView;

        public override void Initialize()
        {
            base.Initialize();
            playerView.Player = Player;
            //playerView.SetPlaybackPreparer();
        }

        protected override void Dispose(bool disposing)
        {
            playerView.OverlayFrameLayout.RemoveAllViews();
            base.Dispose(disposing);
        }
    }*/
}
