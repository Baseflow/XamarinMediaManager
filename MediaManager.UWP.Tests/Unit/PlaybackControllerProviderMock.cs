using Moq;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager.UWP.Tests.Unit
{
    internal class PlaybackControllerProviderMock : Mock<IPlaybackControllerProvider>
    {
        public PlaybackControllerProviderMock SetupGetPlayBackController(IPlaybackController playbackController)
        {
            Setup(mock => mock.PlaybackController).Returns(playbackController);
            return this;
        }
    }
}