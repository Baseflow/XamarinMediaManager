using Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.SystemWrappers;

namespace Plugin.MediaManager.UWP.Tests.Unit
{
    [TestClass]
    public class RemoteControlNotificationHandlerTest
    {
        [TestMethod]
        public void ctor_MediaButtonsAreEnabled()
        {
            var controlsMock = new Mock<ISystemMediaTransportControlsWrapper>();

            new RemoteControlNotificationHandler(controlsMock.Object, new Mock<IPlaybackController>().Object);

            controlsMock.VerifySet(m => m.IsNextEnabled = true);
            controlsMock.VerifySet(m => m.IsPreviousEnabled = true);
            controlsMock.VerifySet(m => m.IsPlayEnabled = true);
        }

        [TestMethod]
        public void SubscribeToNotifications_SubscribeToMediaButtonEventsIsCalled()
        {
            var controlsMock = new Mock<ISystemMediaTransportControlsWrapper>();

            var handler = new RemoteControlNotificationHandler(controlsMock.Object, new Mock<IPlaybackController>().Object);
            handler.SubscribeToNotifications();

            controlsMock.Verify(m => m.SubscribeToMediaButtonEvents());
        }

        [TestMethod]
        public void UnsubscribeFromNotifications_CallsUnsubscribeFromMediaButtonEvents()
        {
            var controlsMock = new Mock<ISystemMediaTransportControlsWrapper>();

            var handler = new RemoteControlNotificationHandler(controlsMock.Object, new Mock<IPlaybackController>().Object);
            handler.UnsubscribeFromNotifications();

            controlsMock.Verify(m => m.UnsubscribeFromMediaButtonEvents());
        }

        [TestMethod]
        public void NextButtonPressed_PlayNextIsCalled()
        {
            var controlsMock = new Mock<ISystemMediaTransportControlsWrapper>();
            var playbackControllerMock = new Mock<IPlaybackController>();

            var handler = new RemoteControlNotificationHandler(controlsMock.Object, playbackControllerMock.Object);
            handler.SubscribeToNotifications();

            var args = new Mock<ISystemMediaTransportControlsButtonPressedEventArgsWrapper>();
            args.SetupGet(m => m.Button).Returns(SystemMediaTransportControlsButton.Next);
            controlsMock.Raise(m => m.ButtonPressed += null, null, args.Object);

            playbackControllerMock.Verify(m => m.PlayNext(), Times.Once);
        }

        [TestMethod]
        public void PreviousButtonPressed_PlayPreviousIsCalled()
        {
            var controlsMock = new Mock<ISystemMediaTransportControlsWrapper>();
            var playbackControllerMock = new Mock<IPlaybackController>();

            var handler = new RemoteControlNotificationHandler(controlsMock.Object, playbackControllerMock.Object);
            handler.SubscribeToNotifications();

            var args = new Mock<ISystemMediaTransportControlsButtonPressedEventArgsWrapper>();
            args.SetupGet(m => m.Button).Returns(SystemMediaTransportControlsButton.Previous);
            controlsMock.Raise(m => m.ButtonPressed += null, null, args.Object);

            playbackControllerMock.Verify(m => m.PlayPrevious(), Times.Once);
        }

        [TestMethod]
        public void PlayButtonPressed_PlayPauseIsCalled()
        {
            var controlsMock = new Mock<ISystemMediaTransportControlsWrapper>();
            var playbackControllerMock = new Mock<IPlaybackController>();

            var handler = new RemoteControlNotificationHandler(controlsMock.Object, playbackControllerMock.Object);
            handler.SubscribeToNotifications();

            var args = new Mock<ISystemMediaTransportControlsButtonPressedEventArgsWrapper>();
            args.SetupGet(m => m.Button).Returns(SystemMediaTransportControlsButton.Play);
            controlsMock.Raise(m => m.ButtonPressed += null, null, args.Object);

            playbackControllerMock.Verify(m => m.PlayPause(), Times.Once);
        }
    }
}
