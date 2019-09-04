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
            switch (mediaSource)
            {
                case string url:
                    return await CrossMediaManager.Current.Play(url);
                case IEnumerable<string> urls:
                    return await CrossMediaManager.Current.Play(urls);
                case IMediaItem mediaItem:
                    return await CrossMediaManager.Current.Play(mediaItem);
                case IEnumerable<IMediaItem> mediaItems:
                    return await CrossMediaManager.Current.Play(mediaItems);
                case FileInfo fileInfo:
                    return await CrossMediaManager.Current.Play(fileInfo);
                case DirectoryInfo directoryInfo:
                    return await CrossMediaManager.Current.Play(directoryInfo);
                case IAlbum album:
                    return await CrossMediaManager.Current.Play(album.MediaItems);
                case IRadio radio:
                    return await CrossMediaManager.Current.Play(radio.MediaItems);
                case IPlaylist playlist:
                    return await CrossMediaManager.Current.Play(playlist.MediaItems);
                case IArtist artist:
                    return await CrossMediaManager.Current.Play(artist.AllTracks);
                default:
                    return null;
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
