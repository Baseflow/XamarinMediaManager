using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Queue;

namespace MediaManager
{
    public static partial class MediaManagerExtensions
    {
        public static async Task Play(this IMediaManager mediaManager, object mediaSource)
        {
            switch (mediaSource)
            {
                case string url:
                    await CrossMediaManager.Current.Play(url);
                    break;
                case IEnumerable<string> urls:
                    await CrossMediaManager.Current.Play(urls);
                    break;
                case IMediaItem mediaItem:
                    await CrossMediaManager.Current.Play(mediaItem);
                    break;
                case IEnumerable<IMediaItem> mediaItems:
                    await CrossMediaManager.Current.Play(mediaItems);
                    break;
                default:
                    break;
            }
        }

        public static Task PlayPreviousOrSeekToStart(this IMediaManager mediaManager)
        {
            if (mediaManager.Position < TimeSpan.FromSeconds(3))
                return mediaManager.PlayPrevious();
            else
                return SeekToStart(mediaManager);
        }

        public static bool IsPlaying(this IMediaManager mediaManager)
        {
            return mediaManager.State == MediaPlayerState.Playing;
        }

        public static bool IsBuffering(this IMediaManager mediaManager)
        {
            return mediaManager.State == MediaPlayerState.Buffering;
        }

        public static bool IsStopped(this IMediaManager mediaManager)
        {
            return mediaManager.State == MediaPlayerState.Stopped || mediaManager.State == MediaPlayerState.Failed;
        }

        public static Task PlayPause(this IMediaManager mediaManager)
        {
            var state = mediaManager.State;

            if (state == MediaPlayerState.Paused || state == MediaPlayerState.Stopped)
                return mediaManager.Play();
            else
                return mediaManager.Pause();
        }

        public static Task SeekToStart(this IMediaManager mediaManager)
        {
            return mediaManager.SeekTo(TimeSpan.Zero);
        }

        /// <summary>
        /// Enables or disables repeat mode
        /// </summary>
        public static void ToggleRepeat(this IMediaManager mediaManager)
        {
            if (mediaManager.RepeatMode == RepeatMode.Off)
            {
                mediaManager.RepeatMode = RepeatMode.All;
            }
            else if (mediaManager.RepeatMode == RepeatMode.All)
            {
                mediaManager.RepeatMode = RepeatMode.One;
            }
            else
            {
                mediaManager.RepeatMode = RepeatMode.Off;
            }
        }

        /// <summary>
        /// Enables or disables shuffling
        /// </summary>
        public static void ToggleShuffle(this IMediaManager mediaManager)
        {
            if (mediaManager.ShuffleMode == ShuffleMode.Off)
            {
                mediaManager.ShuffleMode = ShuffleMode.All;
            }
            else
            {
                mediaManager.ShuffleMode = ShuffleMode.Off;
            }
        }
    }
}
