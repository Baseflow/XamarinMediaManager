using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Library;
using MediaManager.Playback;
using MediaManager.Player;
using MediaManager.Queue;

namespace MediaManager
{
    public static partial class MediaManagerExtensions
    {
        /// <summary>
        /// Tries to Play the mediaSource by checking the type. Returns null when unable to find a playable type
        /// </summary>
        /// <param name="mediaManager"></param>
        /// <param name="mediaSource"></param>
        /// <returns></returns>
        public static async Task<IMediaItem> Play(this IMediaManager mediaManager, object mediaSource)
        {
            IMediaItem mediaItem = null;
            switch (mediaSource)
            {
                case string url:
                    mediaItem = await mediaManager.Play(url);
                    break;
                case IEnumerable<string> urls:
                    mediaItem = await mediaManager.Play(urls);
                    break;
                case IMediaItem media:
                    mediaItem = await mediaManager.Play(media);
                    break;
                case IEnumerable<IMediaItem> mediaItems:
                    mediaItem = await mediaManager.Play(mediaItems);
                    break;
                case FileInfo fileInfo:
                    mediaItem = await mediaManager.Play(fileInfo);
                    break;
                case DirectoryInfo directoryInfo:
                    mediaItem = await mediaManager.Play(directoryInfo);
                    break;
                case IAlbum album:
                    mediaItem = await mediaManager.Play(album.MediaItems);
                    break;
                case IRadio radio:
                    mediaItem = await mediaManager.Play(radio.MediaItems);
                    break;
                case IPlaylist playlist:
                    mediaItem = await mediaManager.Play(playlist.MediaItems);
                    break;
                case IArtist artist:
                    mediaItem = await mediaManager.Play(artist.AllTracks);
                    break;
            }
            return mediaItem;
        }

        public static async Task<IMediaItem> Play(this IMediaManager mediaManager, IMediaItem mediaItem, TimeSpan startAt, TimeSpan? stopAt = null)
        {
            if (mediaManager is MediaManagerBase mediaManagerBase)
                mediaItem = await mediaManagerBase.PrepareQueueForPlayback(mediaItem);

            await mediaManager.MediaPlayer.Play(mediaItem, startAt, stopAt);
            return mediaItem;
        }

        public static Task PlayPreviousOrSeekToStart(this IMediaManager mediaManager, TimeSpan? timeSpan = null)
        {
            timeSpan = timeSpan ?? TimeSpan.FromSeconds(3);

            if (mediaManager.Position < timeSpan)
                return mediaManager.PlayPrevious();
            else
                return SeekToStart(mediaManager);
        }

        public static bool IsPlaying(this IMediaManager mediaManager)
        {
            return mediaManager.State == MediaPlayerState.Playing || mediaManager.State == MediaPlayerState.Buffering;
        }

        public static bool IsPrepared(this IMediaManager mediaManager)
        {
            return mediaManager.State == MediaPlayerState.Playing || mediaManager.State == MediaPlayerState.Paused || mediaManager.State == MediaPlayerState.Buffering;
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
