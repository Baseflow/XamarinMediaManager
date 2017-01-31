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
            _mediaManagerMock = new Mock<IMediaManager>(MockBehavior.Strict);
        }

        [Test]
        public async Task PlayPause_Playing_Pauses()
        {
            _mediaManagerMock
                .Setup(mediaManager => mediaManager.Pause())
                .Returns(Task.FromResult(0));

            MediaManagerStatus = MediaPlayerStatus.Playing;

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.PlayPause();

            _mediaManagerMock
                .Verify(mediaManager => mediaManager.Pause(), Times.Once);
        }

        [Test, TestCaseSource(nameof(NotPlayingStatuses))]
        public async Task PlayPause_NotPlaying_Pauses(MediaPlayerStatus notPlayingStatus)
        {
            _mediaManagerMock
                .Setup(mediaManager => mediaManager.Play((IMediaFile) null))
                .Returns(Task.FromResult(0));

            MediaManagerStatus = notPlayingStatus;

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.PlayPause();

            _mediaManagerMock
                .Verify(mediaManager => mediaManager.Play((IMediaFile) null), Times.Once);
        }

        [Test]
        public async Task PlayPrevious_QueueHasPrevious_PlaysPrevious()
        {
            _mediaManagerMock
                .Setup(mediaManager => mediaManager.PlayPrevious())
                .Returns(Task.FromResult(0));

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

            var playbackControllerMock = new Mock<PlaybackController>(MediaManager) {CallBase = true};

            SetupSeekToStart(playbackControllerMock);

            var playbackController = playbackControllerMock.Object;

            await playbackController.PlayPrevious();

            playbackControllerMock
                .Verify(controller => controller.SeekToStart(), Times.Once);
        }

        [Test, Sequential]
        public async Task PlayPreviousOrSeekToStart(
            [Values(2,4)] int positionSeconds,
            [Values(false, true)] bool afterTreshold
        )
        {
            Duration = TimeSpan.FromSeconds(10);

            if (!afterTreshold)
            {
                _mediaManagerMock
                    .Setup(mediaManager => mediaManager.PlayPrevious())
                    .Returns(Task.FromResult(0));
            }

            MediaQueue = GetMediaQueue(hasPrevious: true);

            Position = TimeSpan.FromSeconds(positionSeconds);

            var playbackControllerMock = new Mock<PlaybackController>(MediaManager) {CallBase = true};

            SetupSeekToStart(playbackControllerMock);

            var playbackController = playbackControllerMock.Object;

            await playbackController.PlayPreviousOrSeekToStart();

            if (afterTreshold)
            {
                playbackControllerMock
                    .Verify(controller => controller.SeekToStart(), Times.Once);
            }
            else
            {
                _mediaManagerMock
                    .Verify(mediaManager => mediaManager.PlayPrevious(), Times.Once);
            }
        }

        [Test]
        public async Task SeekTo_PositionBetweenStartAndEnd_SeeksToPosition()
        {
            var secondsToSeekTo = 5;
            var positionToSeekTo = TimeSpan.FromSeconds(secondsToSeekTo);

            SetupSeek(positionToSeekTo);

            Duration = TimeSpan.FromSeconds(10);

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.SeekTo(secondsToSeekTo);

            _mediaManagerMock
                .Verify(mediaManager => mediaManager.Seek(positionToSeekTo), Times.Once);
        }

        [Test]
        public async Task SeekTo_NegativePosition_SeeksToZero()
        {
            SetupSeek(TimeSpan.Zero);

            Duration = TimeSpan.Zero;

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.SeekTo(-1f);

            _mediaManagerMock
                .Verify(mediaManager => mediaManager.Seek(TimeSpan.Zero), Times.Once);
        }

        [Test]
        public async Task SeekTo_PositionGreaterThanDuration_SeeksToEnd()
        {
            var duration = TimeSpan.FromSeconds(5);

            SetupSeek(duration);

            Duration = duration;

            var playbackController = new PlaybackController(MediaManager);

            await playbackController.SeekTo(10);

            _mediaManagerMock
                .Verify(mediaManager => mediaManager.Seek(duration));
        }

        private void SetupSeekToStart(Mock<PlaybackController> playbackControllerMock)
        {
            playbackControllerMock
                .Setup(playbackController => playbackController.SeekToStart())
                .Returns(Task.FromResult(0));
        }

        private void SetupSeek(TimeSpan timespan)
        {
            _mediaManagerMock
                .Setup(mediaManager => mediaManager.Seek(timespan))
                .Returns(Task.FromResult(0));
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

        private TimeSpan Duration
        {
            set
            {
                var duration = value;

                _mediaManagerMock
                    .SetupGet(mediaManager => mediaManager.Duration)
                    .Returns(duration);
            }
        }

        private TimeSpan Position
        {
            set
            {
                var position = value;

                _mediaManagerMock
                    .SetupGet(mediaManager => mediaManager.Position)
                    .Returns(position);
            }
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