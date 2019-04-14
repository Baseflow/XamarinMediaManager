using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Platforms.Uap.Media;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Video;
using MediaManager.Volume;
using Windows.Media.Playback;

namespace MediaManager
{
    public class MediaManagerImplementation : MediaManagerBase<WindowsMediaPlayer, MediaPlayer>
    {
        public override IMediaPlayer MediaPlayer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IMediaExtractor MediaExtractor { get => _MediaExtractor; set => _MediaExtractor = value; }
        public override IVolumeManager VolumeManager { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // - - -  - - - 

        public override Playback.MediaPlayerState State => GetMediaPlayerState();

        private Playback.MediaPlayerState GetMediaPlayerState()
        {
            //ToDo: ME:  Stopped ?, Loading ?, Failed ?

            switch (_player.PlaybackSession.PlaybackState)
            {
                case MediaPlaybackState.Buffering: return Playback.MediaPlayerState.Buffering;
                case MediaPlaybackState.None: return Playback.MediaPlayerState.Stopped;
                case MediaPlaybackState.Opening: return Playback.MediaPlayerState.Loading;
                case MediaPlaybackState.Paused: return Playback.MediaPlayerState.Paused;
                case MediaPlaybackState.Playing: return Playback.MediaPlayerState.Playing;
            };

            return Playback.MediaPlayerState.Paused;
        }
        // - - -  - - - 

        public override TimeSpan Position => _player.PlaybackSession.Position;

        public override TimeSpan Duration => _player.PlaybackSession.NaturalDuration;

        public override TimeSpan Buffered => throw new NotImplementedException();

        public override float Speed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override RepeatMode RepeatMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override ShuffleMode ShuffleMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // - - -  - - - 

        private readonly MediaPlayer _player;
        private IMediaExtractor _MediaExtractor;

        public MediaManagerImplementation()
        {
            _player = new MediaPlayer();
            _MediaExtractor = new Platforms.Uap.UapMediaExtractor();
        }

        // - - -  - - - 

        public override void Init()
        {
            //ToDo: ME
            IsInitialized = true;
        }

        public override Task Pause()
        {
            throw new NotImplementedException();
        }

        public override Task Play(IMediaItem mediaItem)
        {
            throw new NotImplementedException();
        }

        public override async Task<IMediaItem> Play(string uri)
        {
            var mediaItem = await MediaExtractor.CreateMediaItem(uri);

            //ToDo: ME
            _player.SetUriSource( new Uri(mediaItem.MediaUri) );
            _player.Play(); // ? autoplay
            // _player.Source = new MediaPlaybackItem(new Uri(uri));

            return mediaItem;
        }

        public override Task Play(IEnumerable<IMediaItem> items)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
        {
            throw new NotImplementedException();
        }

        public override Task<IMediaItem> Play(FileInfo file)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo)
        {
            throw new NotImplementedException();
        }

        public override Task Play()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> PlayNext()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> PlayPrevious()
        {
            throw new NotImplementedException();
        }

        public override Task SeekTo(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public override Task StepBackward()
        {
            throw new NotImplementedException();
        }

        public override Task StepForward()
        {
            throw new NotImplementedException();
        }

        public override Task Stop()
        {
            throw new NotImplementedException();
        }
    }
}
