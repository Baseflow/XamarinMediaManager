using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Platforms.Uap.Media;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Volume;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace MediaManager
{
    public class MediaManagerImplementation : MediaManagerBase<WindowsMediaPlayer, MediaPlayer>
    {
        public override IMediaPlayer MediaPlayer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IMediaExtractor MediaExtractor { get => _MediaExtractor; set => _MediaExtractor = value; }
        public override IVolumeManager VolumeManager { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // - - -  - - - 

        public override Playback.MediaPlayerState State
        {
            get { return _State; }
            //ToDo: ME discuss with Martijn
            //private set
            //{
            //    _state = value;
            //    MediaManager.OnStateChanged(this, new StateChangedEventArgs(_state));
            //}
        }
        private Playback.MediaPlayerState _State;


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
            //ToDo: ME - reorg

            _player.CurrentStateChanged += (MediaPlayer sender, object args) =>
            {
                _State = GetMediaPlayerState();
                this.OnStateChanged(this, new StateChangedEventArgs(GetMediaPlayerState()));
            };

            //ToDo: ME - todo
            //event PlayingChangedEventHandler PlayingChanged;
            //event BufferingChangedEventHandler BufferingChanged;

            //event MediaItemFinishedEventHandler MediaItemFinished;
            //event MediaItemChangedEventHandler MediaItemChanged;

            _player.SourceChanged += (MediaPlayer sender, object args) =>
            {
                this.OnMediaItemChanged(this, new MediaItemEventArgs(this.MediaQueue.Current));
            };

            _player.MediaFailed += (MediaPlayer sender, MediaPlayerFailedEventArgs args) =>
            {
                _State = Playback.MediaPlayerState.Failed;
                _player.PlaybackSession.Position = TimeSpan.Zero;
                this.OnMediaItemFailed(this, new MediaItemFailedEventArgs(this.MediaQueue.Current, args.ExtendedErrorCode, args.ErrorMessage));
            };

            IsInitialized = true;
        }

        public override Task Pause()
        {
            if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
            {
                _player.Play();
            }
            else
            {
                //ToDo: ME: Why not a play from zéro?
                _player.Pause();
            };

            return Task.CompletedTask;
        }

        public override Task Play(IMediaItem mediaItem)
        {
            throw new NotImplementedException();
        }

        // - - -  - - - 

        public override async Task<IMediaItem> Play(string uri)
        {
            var mediaItem = await MediaExtractor.CreateMediaItem(uri);

            var mediaPlaybackList = new MediaPlaybackList();
            var mediaSource = await CreateMediaSource(mediaItem);
            var item = new MediaPlaybackItem(mediaSource);
            mediaPlaybackList.Items.Add(item);
            _player.Source = mediaPlaybackList;
            _player.Play();

            return mediaItem;
        }

        private async Task<MediaSource> CreateMediaSource(IMediaItem mediaItem)
        {
            switch (mediaItem.MediaLocation)
            {
                case MediaLocation.Remote:
                    return MediaSource.CreateFromUri(new Uri(mediaItem.MediaUri));

                case MediaLocation.FileSystem:
                    var du = _player.SystemMediaTransportControls.DisplayUpdater;
                    var storageFile = await StorageFile.GetFileFromPathAsync(mediaItem.MediaUri);
                    var playbackType = (mediaItem.MediaType == MediaType.Audio ? Windows.Media.MediaPlaybackType.Music : Windows.Media.MediaPlaybackType.Video);
                    await du.CopyFromFileAsync(playbackType, storageFile);
                    du.Update();
                    return MediaSource.CreateFromStorageFile(storageFile);
            }

            return MediaSource.CreateFromUri(new Uri(mediaItem.MediaUri));
        }

        // - - -  - - - 

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
            _player.PlaybackSession.PlaybackRate = 1;
            _player.Play();
            return Task.CompletedTask;
        }

        public override Task<bool> PlayNext()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> PlayPrevious()
        {
            throw new NotImplementedException();
        }

        public override async Task SeekTo(TimeSpan position)
        {
            _player.PlaybackSession.Position = position;
            await Task.CompletedTask;
        }

        //public override Task StepBackward()
        //{
        //    throw new NotImplementedException();
        //}

        //public override Task StepForward()
        //{
        //    throw new NotImplementedException();
        //}

        public override Task Stop()
        {
            _player.PlaybackSession.PlaybackRate = 0;
            _player.PlaybackSession.Position = TimeSpan.Zero;

            _State = Playback.MediaPlayerState.Stopped;
            this.OnStateChanged(this, new StateChangedEventArgs(_State));

            return Task.CompletedTask;
        }
    }
}
