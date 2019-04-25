using System;
using Android.OS;
using Android.Runtime;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;

namespace MediaManager.Platforms.Android.Media
{
    public class PlaybackController : DefaultPlaybackController
    {
        public PlaybackController()
        {
        }

        protected PlaybackController(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public PlaybackController(long rewindIncrementMs, long fastForwardIncrementMs, int repeatToggleModes) : base(rewindIncrementMs, fastForwardIncrementMs, repeatToggleModes)
        {
        }

        public override string[] GetCommands()
        {
            return base.GetCommands();
        }

        public override long GetSupportedPlaybackActions(IPlayer player)
        {
            return base.GetSupportedPlaybackActions(player);
        }

        public override void OnCommand(IPlayer player, string command, Bundle extras, ResultReceiver cb)
        {
            base.OnCommand(player, command, extras, cb);
        }

        public override void OnFastForward(IPlayer player)
        {
            base.OnFastForward(player);
        }

        public override void OnPause(IPlayer player)
        {
            base.OnPause(player);
        }

        public override void OnPlay(IPlayer player)
        {
            base.OnPlay(player);
        }

        public override void OnRewind(IPlayer player)
        {
            base.OnRewind(player);
        }

        public override void OnSeekTo(IPlayer player, long position)
        {
            base.OnSeekTo(player, position);
        }

        public override void OnSetRepeatMode(IPlayer player, int repeatMode)
        {
            base.OnSetRepeatMode(player, repeatMode);
        }

        public override void OnSetShuffleMode(IPlayer player, int shuffleMode)
        {
            base.OnSetShuffleMode(player, shuffleMode);
        }

        public override void OnStop(IPlayer player)
        {
            base.OnStop(player);
        }
    }
}
