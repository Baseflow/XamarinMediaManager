using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using MediaManager.Media;
using MediaManager.Platforms.Android;
using MediaManager.Platforms.Android.Media;
using MediaManager.Platforms.Android.MediaSession;
using MediaManager.Platforms.Android.Playback;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Volume;

[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessWifiState)]
[assembly: UsesPermission(Android.Manifest.Permission.ForegroundService)]
namespace MediaManager
{
    [global::Android.Runtime.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : MediaManagerBase, IMediaManager<SimpleExoPlayer>
    {
        public MediaManagerImplementation()
        {
        }

        private Context _context = Application.Context;
        public virtual Context Context
        {
            get => _context;
            set
            {
                if (SetProperty(ref _context, value))
                    SessionActivityPendingIntent = BuildSessionActivityPendingIntent();
            }
        }

        private int _notificationIconResource = Resource.Drawable.exo_notification_play;
        public int NotificationIconResource
        {
            get => _notificationIconResource;
            set => SetProperty(ref _notificationIconResource, value);
        }

        private MediaSessionCompat _mediaSession;
        public MediaSessionCompat MediaSession
        {
            get => _mediaSession;
            set => SetProperty(ref _mediaSession, value);
        }

        private PendingIntent _sessionActivityPendingIntent;
        public virtual PendingIntent SessionActivityPendingIntent
        {
            get
            {
                if (_sessionActivityPendingIntent == null)
                {
                    _sessionActivityPendingIntent = BuildSessionActivityPendingIntent();
                }
                return _sessionActivityPendingIntent;
            }
            set => SetProperty(ref _sessionActivityPendingIntent, value);
        }

        public virtual PendingIntent BuildSessionActivityPendingIntent()
        {
            Intent sessionIntent;
            // Build a PendingIntent that can be used to launch the UI.
            if (Context is Activity activity)
                sessionIntent = new Intent(Context, activity.GetType());
            else
                sessionIntent = Context.PackageManager.GetLaunchIntentForPackage(Context.PackageName);
            return PendingIntent.GetActivity(Context, 0, sessionIntent, 0);
        }

        public override void Init()
        {
            IsInitialized = MediaBrowserManager.Init();
        }

        private MediaBrowserManager _mediaBrowserManager;
        public virtual MediaBrowserManager MediaBrowserManager
        {
            get
            {
                if (_mediaBrowserManager == null)
                    _mediaBrowserManager = new MediaBrowserManager();
                return _mediaBrowserManager;
            }
            set => SetProperty(ref _mediaBrowserManager, value);
        }

        private IMediaPlayer _mediaPlayer;
        public override IMediaPlayer MediaPlayer
        {
            get
            {
                if (_mediaPlayer == null)
                    _mediaPlayer = new AndroidMediaPlayer();
                return _mediaPlayer;
            }
            set => SetProperty(ref _mediaPlayer, value);
        }

        public AndroidMediaPlayer AndroidMediaPlayer => (AndroidMediaPlayer)MediaPlayer;
        public SimpleExoPlayer Player => AndroidMediaPlayer.Player;

        private IVolumeManager _volumeManager;
        public override IVolumeManager VolumeManager
        {
            get
            {
                if (_volumeManager == null)
                    _volumeManager = new VolumeManager(this);
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
                    _notificationManager = new MediaManager.Platforms.Android.Notifications.NotificationManager();

                return _notificationManager;
            }
            set => SetProperty(ref _notificationManager, value);
        }

        public override TimeSpan Position => TimeSpan.FromMilliseconds(MediaBrowserManager?.MediaController.PlaybackState?.Position ?? 0);

        public override TimeSpan Duration => MediaBrowserManager?.MediaController.Metadata?.ToMediaItem().Duration ?? TimeSpan.Zero;

        public override float Speed {
            get => MediaBrowserManager?.MediaController.PlaybackState?.PlaybackSpeed ?? 0;
            set => throw new NotImplementedException();
        }

        public override Task Pause()
        {
            MediaBrowserManager.MediaController.GetTransportControls().Pause();
            return Task.CompletedTask;
        }

        public override Task Play()
        {
            if(this.IsStopped())
                MediaBrowserManager.MediaController.GetTransportControls().Prepare();

            MediaBrowserManager.MediaController.GetTransportControls().Play();
            return Task.CompletedTask;
        }

        public override async Task<IMediaItem> Play(string uri)
        {
            var mediaItem = await MediaExtractor.CreateMediaItem(uri);
            await AddMediaItemsToQueue(new List<IMediaItem> { mediaItem }, true);

            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return mediaItem;
        }

        public override async Task Play(IMediaItem mediaItem)
        {
            await AddMediaItemsToQueue(new List<IMediaItem> { mediaItem }, true);

            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return;
        }

        public override async Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
        {
            var mediaItems = new List<IMediaItem>();
            foreach (var uri in items)
            {
                mediaItems.Add(await MediaExtractor.CreateMediaItem(uri));
            }

            await AddMediaItemsToQueue(mediaItems, true);

            await MediaQueue.FirstOrDefault()?.FetchMetaData();
            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return MediaQueue;
        }

        public override async Task Play(IEnumerable<IMediaItem> items)
        {
            await AddMediaItemsToQueue(items, true);

            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return;
        }

        public override async Task<IMediaItem> Play(FileInfo file)
        {
            var mediaItem = await MediaExtractor.CreateMediaItem(file);
            var mediaItemToPlay = await AddMediaItemsToQueue(new List<IMediaItem> { mediaItem }, true);

            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return mediaItem;
        }

        public override async Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo)
        {
            var mediaItems = new List<IMediaItem>();
            foreach (var file in directoryInfo.GetFiles())
            {
                var mediaItem = await MediaExtractor.CreateMediaItem(file);
                mediaItems.Add(mediaItem);
            }

            await AddMediaItemsToQueue(mediaItems, true);

            await MediaQueue.FirstOrDefault()?.FetchMetaData();
            MediaBrowserManager.MediaController.GetTransportControls().Prepare();
            return MediaQueue;
        }

        public override Task<bool> PlayNext()
        {
            if (AndroidMediaPlayer.Player.NextWindowIndex == AndroidMediaPlayer.Player.CurrentWindowIndex)
            {
                SeekTo(TimeSpan.FromSeconds(0));
                return Task.FromResult(true);
            }

            if (AndroidMediaPlayer.Player.NextWindowIndex == -1)
            {
                return Task.FromResult(false);
            }

            MediaBrowserManager.MediaController.GetTransportControls().SkipToNext();

            return Task.FromResult(true);
        }

        public override Task<bool> PlayPrevious()
        {
            if (AndroidMediaPlayer.Player.PreviousWindowIndex == AndroidMediaPlayer.Player.CurrentWindowIndex)
            {
                SeekTo(TimeSpan.FromSeconds(0));
                return Task.FromResult(true);
            }

            if (AndroidMediaPlayer.Player.PreviousWindowIndex == -1)
            {
                return Task.FromResult(false);
            }

            MediaBrowserManager.MediaController.GetTransportControls().SkipToPrevious();

            return Task.FromResult(true);
        }

        public override Task<bool> PlayQueueItem(IMediaItem mediaItem)
        {
            if(!MediaQueue.Contains(mediaItem))
                return Task.FromResult(false);

            MediaBrowserManager.MediaController.GetTransportControls().SkipToQueueItem(MediaQueue.IndexOf(mediaItem));
            return Task.FromResult(true);
        }

        public override Task SeekTo(TimeSpan position)
        {
            MediaBrowserManager.MediaController.GetTransportControls().SeekTo((long)position.TotalMilliseconds);
            return Task.CompletedTask;
        }

        public override Task Stop()
        {
            MediaBrowserManager.MediaController.GetTransportControls().Stop();
            (NotificationManager as MediaManager.Platforms.Android.Notifications.NotificationManager).Player = null;
            return Task.CompletedTask;
        }

        public override RepeatMode RepeatMode
        {
            get
            {
                return AndroidMediaPlayer.RepeatMode;
            }
            set
            {
                MediaBrowserManager.MediaController.GetTransportControls().SetRepeatMode((int)value);
            }
        }

        public override ShuffleMode ShuffleMode
        {
            get
            {
                return (ShuffleMode)MediaBrowserManager.MediaController.ShuffleMode;
            }
            set
            {
                MediaBrowserManager.MediaController.GetTransportControls().SetShuffleMode((int)value);
            }
        }
    }
}
