using System;
using Windows.Media.Playback;
using Plugin.MediaManager.Interfaces;

namespace Plugin.MediaManager
{
    public class BasePlayerImplementation : IDisposable
    {
        private readonly IMediaPlyerPlaybackController mediaPlyerPlaybackController;

        protected readonly MediaPlayer Player;

        public BasePlayerImplementation(IMediaPlyerPlaybackController mediaPlyerPlaybackController)
        {
            this.mediaPlyerPlaybackController = mediaPlyerPlaybackController;

            Player = mediaPlyerPlaybackController.Player;
        }

        public void Dispose()
        {
            mediaPlyerPlaybackController?.Dispose();
        }
    }
}
