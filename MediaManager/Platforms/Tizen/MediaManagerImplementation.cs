using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Platforms.Tizen;
using MediaManager.Platforms.Tizen.Media;
using MediaManager.Platforms.Tizen.Volume;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Volume;
using Tizen.Multimedia;

namespace MediaManager
{
    public class MediaManagerImplementation : MediaManagerBase, IMediaManager<Player>
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
                    _mediaPlayer = new TizenMediaPlayer();
                return _mediaPlayer;
            }
            set => SetProperty(ref _mediaPlayer, value);
        }

        public TizenMediaPlayer TizenMediaPlayer => (TizenMediaPlayer)MediaPlayer;
        public Player Player => TizenMediaPlayer.Player;

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
                    _notificationManager = new MediaManager.Platforms.Tizen.Notifications.NotificationManager();

                return _notificationManager;
            }
            set => SetProperty(ref _notificationManager, value);
        }

        public override Task Pause()
        {
            return MediaPlayer.Pause();
        }

        public override async Task Play(IMediaItem mediaItem)
        {
            var mediaItemToPlay = await AddMediaItemsToQueue(new List<IMediaItem> { mediaItem }, true);

            await MediaPlayer.Play(mediaItemToPlay);
        }

        public override async Task<IMediaItem> Play(string uri)
        {
            var mediaItem = await MediaExtractor.CreateMediaItem(uri);
            var mediaItemToPlay = await AddMediaItemsToQueue(new List<IMediaItem> { mediaItem }, true);

            await MediaPlayer.Play(mediaItemToPlay);
            return mediaItem;
        }

        public override async Task Play(IEnumerable<IMediaItem> items)
        {
            var mediaItemToPlay = await AddMediaItemsToQueue(items, true);

            await MediaPlayer.Play(mediaItemToPlay);
        }

        public override async Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
        {
            var mediaItems = new List<IMediaItem>();
            foreach (var uri in items)
            {
                mediaItems.Add(await MediaExtractor.CreateMediaItem(uri));
            }

            var mediaItemToPlay = await AddMediaItemsToQueue(mediaItems, true);
            await MediaPlayer.Play(mediaItemToPlay);
            return MediaQueue;
        }

        public override async Task<IMediaItem> Play(FileInfo file)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo)
        {
            throw new NotImplementedException();
        }

        public override Task Play()
        {
            return MediaPlayer.Play();
        }

        public override Task SeekTo(TimeSpan position)
        {
            return MediaPlayer.SeekTo(position);
        }

        public override Task Stop()
        {
            return MediaPlayer.Stop();
        }
        public override RepeatMode RepeatMode
        {
            get; set;
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

        public override TimeSpan Position => TimeSpan.FromMilliseconds(Player.GetPlayPosition());

        public override TimeSpan Duration => TimeSpan.Zero;

        public override float Speed { get; set; }
    }
}
