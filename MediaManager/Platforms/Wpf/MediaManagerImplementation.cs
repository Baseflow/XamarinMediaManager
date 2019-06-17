using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Platforms.Wpf.Media;
using MediaManager.Platforms.Wpf.Notificiations;
using MediaManager.Platforms.Wpf.Player;
using MediaManager.Platforms.Wpf.Volume;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager
{
    public class MediaManagerImplementation : MediaManagerBase
    {
        public MediaManagerImplementation()
        {

        }

        private IMediaPlayer _mediaPlayer;
        public override IMediaPlayer MediaPlayer
        {
            get
            {
                if (_mediaPlayer == null)
                    _mediaPlayer = new WpfMediaPlayer();
                return _mediaPlayer;
            }
            set => SetProperty(ref _mediaPlayer, value);
        }

        public WpfMediaPlayer NetMediaPlayer => (WpfMediaPlayer)MediaPlayer;

        private IVolumeManager _volumeManager;
        public override IVolumeManager VolumeManager
        {
            get
            {
                if (_volumeManager == null)
                    _volumeManager = new VolumeManager();
                return _volumeManager;
            }
            set => SetProperty(ref _volumeManager, value);
        }

        private IMediaExtractor _mediaExtractor;
        public override IMediaExtractor MediaExtractor
        {
            get
            {
                if (_mediaExtractor == null)
                    _mediaExtractor = new MediaExtractor();
                return _mediaExtractor;
            }
            set => SetProperty(ref _mediaExtractor, value);
        }


        private INotificationManager _notificationManager;
        public override INotificationManager NotificationManager
        {
            get
            {
                if (_notificationManager == null)
                    _notificationManager = new NotificationManager();

                return _notificationManager;
            }
            set => SetProperty(ref _notificationManager, value);
        }

        public override TimeSpan Position => NetMediaPlayer?.Player?.Position ?? TimeSpan.Zero;

        public override TimeSpan Duration => NetMediaPlayer?.Player?.NaturalDuration.TimeSpan ?? TimeSpan.Zero;

        public override float Speed
        {
            get
            {
                return 0.0f;
            }
            set
            {
            }
        }

        public override RepeatMode RepeatMode
        {
            get => MediaPlayer.RepeatMode;
            set => MediaPlayer.RepeatMode = value;
        }

        public override ShuffleMode ShuffleMode
        {
            get
            {
                return MediaQueue.ShuffleMode;
            }
            set
            {
                MediaQueue.ShuffleMode = value;
            }
        }

        public override void Init()
        {
            IsInitialized = true;
        }

        public override Task Play()
        {
            return MediaPlayer.Play();
        }

        public override Task Pause()
        {
            return MediaPlayer.Pause();
        }

        public override Task Stop()
        {
            return MediaPlayer.Stop();
        }

        public override Task SeekTo(TimeSpan position)
        {
            return MediaPlayer.SeekTo(position);
        }

        public override Task Play(IMediaItem mediaItem)
        {
            return MediaPlayer.Play(mediaItem);
        }

        public override async Task<IMediaItem> Play(string uri)
        {
            var mediaItem = await MediaExtractor.CreateMediaItem(uri);
            await MediaPlayer.Play(mediaItem);
            return mediaItem;
        }

        public override async Task Play(IEnumerable<IMediaItem> items)
        {
            var mediaItem = await AddMediaItemsToQueue(items, true);
            await MediaPlayer.Play(mediaItem);
        }

        public override async Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
        {
            var mediaItems = new List<IMediaItem>();
            foreach (var uri in items)
            {
                mediaItems.Add(await MediaExtractor.CreateMediaItem(uri));
            }
            var mediaItem = await AddMediaItemsToQueue(mediaItems, true);
            await MediaPlayer.Play(mediaItem);
            return mediaItems;
        }

        public override Task<IMediaItem> Play(FileInfo file)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo)
        {
            throw new NotImplementedException();
        }
    }
}
