using System;
using System.Collections.Generic;
using System.Text;
using Android.Runtime;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;

namespace MediaManager.Platforms.Android.Audio
{
    public class PlaybackController : DefaultPlaybackController
    {
        private AudioFocusManager audioFocusManager;

        public PlaybackController()
        {
        }

        public PlaybackController(AudioFocusManager audioFocusManager)
        {
            this.audioFocusManager = audioFocusManager;
        }

        public PlaybackController(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public PlaybackController(long rewindIncrementMs, long fastForwardIncrementMs, int repeatToggleModes) : base(rewindIncrementMs, fastForwardIncrementMs, repeatToggleModes)
        {
        }

        public override void OnPause(IPlayer player)
        {
            base.OnPause(player);
            audioFocusManager.AbandonAudioFocus();
        }

        public override void OnPlay(IPlayer player)
        {
            base.OnPlay(player);
            audioFocusManager.RequestAudioFocus();
        }

        public override void OnStop(IPlayer player)
        {
            base.OnStop(player);
            audioFocusManager.AbandonAudioFocus();
            //player.Stop();
        }
    }
}
