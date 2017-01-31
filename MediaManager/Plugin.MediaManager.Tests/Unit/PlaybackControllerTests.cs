using System;
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
        public async Task PlayPause_NotPlaying_Pauses(MediaPlayerStatus notPlayingStatus)
        {
            MediaManagerStatus = notPlayingStatus;

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.PlayPause();

            _mediaManagerMock
                .Verify(mediaManager => mediaManager.Play((IMediaFile) null), Times.Once);
        }

        [Test]
        public async Task PlayPrevious_QueueHasPrevious_PlaysPrevious()
        {
            MediaQueue = GetMediaQueue(hasPrevious: true);

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.PlayPrevious();

            _mediaManagerMock
                .Verify(mediaManager => mediaManager.PlayPrevious(), Times.Once);
        }

        [Test]
        public async Task PlayPrevious_QueueHasNoPrevious_SeeksToStart()
        {
            MediaQueue = GetMediaQueue(hasPrevious: false);

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.PlayPrevious();

            _mediaManagerMock
                .Verify(mediaManager => mediaManager.Seek(TimeSpan.Zero), Times.Once);
        }

        private IMediaQueue GetMediaQueue(bool hasPrevious)
        {
            var mediaQueueMock = new Mock<IMediaQueue>();
            mediaQueueMock
                .Setup(queue => queue.HasPrevious())
                .Returns(hasPrevious);

            var mediaQueue = mediaQueueMock.Object;

            return mediaQueue;
        }

        private IMediaQueue MediaQueue
        {
            set
            {
                var queue = value;

                _mediaManagerMock
                    .SetupGet(mediaManager => mediaManager.MediaQueue)
                    .Returns(queue);
            }
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