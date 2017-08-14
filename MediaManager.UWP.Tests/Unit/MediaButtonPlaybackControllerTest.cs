using Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.SystemWrappers;

namespace Plugin.MediaManager.UWP.Tests.Unit
{
    [TestClass]
    public class MediaButtonPlaybackControllerTest
    {
        [TestMethod]
        public void ctor_MediaButtonsAreEnabled()
        {
            var controlsMock = new Mock<ISystemMediaTransportControlsWrapper>();

            new MediaButtonPlaybackController(controlsMock.Object, new PlaybackControllerProviderMock().Object);

            controlsMock.VerifySet(m => m.IsNextEnabled = true);
            controlsMock.VerifySet(m => m.IsPreviousEnabled = true);
            controlsMock.VerifySet(m => m.IsPlayEnabled = true);
            controlsMock.VerifySet(m => m.IsPauseEnabled = true);
            controlsMock.VerifySet(m => m.IsStopEnabled = true);
        }

        [TestMethod]
        public void SubscribeToNotifications_SubscribeToMediaButtonEventsIsCalled()
        {
            var controlsMock = new Mock<ISystemMediaTransportControlsWrapper>();

            var controller = new MediaButtonPlaybackController(controlsMock.Object, new PlaybackControllerProviderMock().Object);
            controller.SubscribeToNotifications();

            controlsMock.Verify(m => m.SubscribeToMediaButtonEvents());
        }

        [TestMethod]
        public void UnsubscribeFromNotifications_CallsUnsubscribeFromMediaButtonEvents()
        {
            var controlsMock = new Mock<ISystemMediaTransportControlsWrapper>();

            var controller = new MediaButtonPlaybackController(controlsMock.Object, new PlaybackControllerProviderMock().Object);
            controller.UnsubscribeFromNotifications();

            controlsMock.Verify(m => m.UnsubscribeFromMediaButtonEvents());
        }

        [TestMethod]
        public void NextButtonPressed_PlayNextIsCalled()
        {
            var controlsMock = new Mock<ISystemMediaTransportControlsWrapper>();
            var playbackControllerMock = new Mock<IPlaybackController>();
            var playbackControllerProviderMock = new PlaybackControllerProviderMock().SetupGetPlayBackController(playbackControllerMock.Object);

            var controller = new MediaButtonPlaybackController(controlsMock.Object, playbackControllerProviderMock.Object);
            controller.SubscribeToNotifications();

            var args = new Mock<ISystemMediaTransportControlsButtonPressedEventArgsWrapper>();
            args.SetupGet(m => m.Button).Returns(SystemMediaTransportControlsButton.Next);
            controlsMock.Raise(m => m.ButtonPressed += null, null, args.Object);

            playbackControllerMock.Verify(m => m.PlayNext(), Times.Once);
        }

        [TestMethod]
        public void PreviousButtonPressed_PlayPreviousOrSeekToStartIsCalled()
        {
            var controlsMock = new Mock<ISystemMediaTransportControlsWrapper>();
            var playbackControllerMock = new Mock<IPlaybackController>();
            var playbackControllerProviderMock = new PlaybackControllerProviderMock().SetupGetPlayBackController(playbackControllerMock.Object);

            var controller = new MediaButtonPlaybackController(controlsMock.Object, playbackControllerProviderMock.Object);
            controller.SubscribeToNotifications();

            var args = new Mock<ISystemMediaTransportControlsButtonPressedEventArgsWrapper>();
            args.SetupGet(m => m.Button).Returns(SystemMediaTransportControlsButton.Previous);
            controlsMock.Raise(m => m.ButtonPressed += null, null, args.Object);

            playbackControllerMock.Verify(m => m.PlayPreviousOrSeekToStart(), Times.Once);
        }

        [TestMethod]
        public void PlayButtonPressed_PlayIsCalled()
        {
            var controlsMock = new Mock<ISystemMediaTransportControlsWrapper>();
            var playbackControllerMock = new Mock<IPlaybackController>();
            var playbackControllerProviderMock = new PlaybackControllerProviderMock().SetupGetPlayBackController(playbackControllerMock.Object);

            var controller = new MediaButtonPlaybackController(controlsMock.Object, playbackControllerProviderMock.Object);
            controller.SubscribeToNotifications();

            var args = new Mock<ISystemMediaTransportControlsButtonPressedEventArgsWrapper>();
            args.SetupGet(m => m.Button).Returns(SystemMediaTransportControlsButton.Play);
            controlsMock.Raise(m => m.ButtonPressed += null, null, args.Object);

            playbackControllerMock.Verify(m => m.Play(), Times.Once);
        }

        [TestMethod]
        public void PauseButtonPressed_PauseIsCalled()
        {
            var controlsMock = new Mock<ISystemMediaTransportControlsWrapper>();
            var playbackControllerMock = new Mock<IPlaybackController>();
            var playbackControllerProviderMock = new PlaybackControllerProviderMock().SetupGetPlayBackController(playbackControllerMock.Object);

            var controller = new MediaButtonPlaybackController(controlsMock.Object, playbackControllerProviderMock.Object);
            controller.SubscribeToNotifications();

            var args = new Mock<ISystemMediaTransportControlsButtonPressedEventArgsWrapper>();
            args.SetupGet(m => m.Button).Returns(SystemMediaTransportControlsButton.Pause);
            controlsMock.Raise(m => m.ButtonPressed += null, null, args.Object);

            playbackControllerMock.Verify(m => m.Pause(), Times.Once);
        }

        [TestMethod]
        public void StopButtonPressed_StopIsCalled()
        {
            var controlsMock = new Mock<ISystemMediaTransportControlsWrapper>();
            var playbackControllerMock = new Mock<IPlaybackController>();
            var playbackControllerProviderMock = new PlaybackControllerProviderMock().SetupGetPlayBackController(playbackControllerMock.Object);

            var controller = new MediaButtonPlaybackController(controlsMock.Object, playbackControllerProviderMock.Object);
            controller.SubscribeToNotifications();

            var args = new Mock<ISystemMediaTransportControlsButtonPressedEventArgsWrapper>();
            args.SetupGet(m => m.Button).Returns(SystemMediaTransportControlsButton.Stop);
            controlsMock.Raise(m => m.ButtonPressed += null, null, args.Object);

            playbackControllerMock.Verify(m => m.Stop(), Times.Once);
        }
    }
}
