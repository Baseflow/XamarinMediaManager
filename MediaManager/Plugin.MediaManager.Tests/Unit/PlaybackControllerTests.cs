using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager.Tests.Unit
{
    [TestFixture]
    public class PlaybackControllerTests
    {
        private Mock<IMediaManager> _mediaManagerMock;
        private IMediaManager MediaManager => _mediaManagerMock.Object;

        [SetUp]
        public void Init()
        {
            _mediaManagerMock = new Mock<IMediaManager>();
        }

        [Test]
        public async Task PlayPause_Playing_Pauses()
        {
            MediaManagerStatus = MediaPlayerStatus.Playing;

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.PlayPause();

            _mediaManagerMock
                .Verify(mediaManager => mediaManager.Pause(), Times.Once);
        }

        [Test, TestCaseSource(nameof(NotPlayingStatuses))]
        public async Task PlayPuase_NotPlaying_Pauses(MediaPlayerStatus notPlayingStatus)
        {
            MediaManagerStatus = notPlayingStatus;

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.PlayPause();

            _mediaManagerMock
                .Verify(mediaManager => mediaManager.Play((IMediaFile) null), Times.Once);
        }

        private MediaPlayerStatus MediaManagerStatus
        {
            set
            {
                var status = value;

                _mediaManagerMock
                    .SetupGet(mediaManager => mediaManager.Status)
                    .Returns(status);
            }
        }

        public static IEnumerable NotPlayingStatuses
        {
            get
            {
                var notPlayingStatuses = new List<MediaPlayerStatus>
                {
                    MediaPlayerStatus.Paused,
                    MediaPlayerStatus.Stopped
                };

                foreach (var status in notPlayingStatuses)
                {
                    yield return new TestCaseData(status);
                }
            }
        }
    }
}