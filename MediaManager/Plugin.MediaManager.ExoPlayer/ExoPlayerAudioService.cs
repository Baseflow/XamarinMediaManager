using System;
using System.Threading.Tasks;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Mediacodec;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager.ExoPlayer
{
    public class ExoPlayerAudioService : MediaPlayerService.MediaPlayerService
    {
        private IExoPlayer mediaPlayer;

        public override async Task Play(IMediaFile mediaFile = null)
        {
            //TODO: Implement ExoPlayer here
            await base.Play(mediaFile);
        }
    }
}
