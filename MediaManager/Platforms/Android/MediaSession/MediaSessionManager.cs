using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions;
using System;
using Android.Support.V4.App;

namespace Plugin.MediaManager.MediaSession
{
    public class MediaSessionManager
    {
        private Context applicationContext;
        private MediaControllerCompat mediaControllerCompat;
        private MediaSessionCompat mediaSessionCompat;
        private MediaServiceBinder _binder;
        private string _packageName;
        private IMediaNotificationManager _notificationManager;
        private static IMediaNotificationManager _overrideNotificationManager;

        public IMediaNotificationManager NotificationManager
        {
            get => _overrideNotificationManager ?? _notificationManager;
            set
            {
                _overrideNotificationManager = value;
                _notificationManager = value;
                UpdateAndroidNotificationManagerSettings();
            }
        }

        private void UpdateAndroidNotificationManagerSettings()
        {
            if (_notificationManager != null)
            {
                ((IAndroidMediaNotificationManager)_notificationManager).SessionToken = CurrentSession?.SessionToken;
                ((IAndroidMediaNotificationManager)_notificationManager).MediaQueue = mediaQueue;
            }

            if (_overrideNotificationManager != null)
            {
                ((IAndroidMediaNotificationManager)_overrideNotificationManager).SessionToken = CurrentSession?.SessionToken;
                ((IAndroidMediaNotificationManager)_overrideNotificationManager).MediaQueue = mediaQueue;
            }
        }

        public event EventHandler<string> OnNotificationActionFired;

        internal event EventHandler<int> OnStatusChanged;

        internal int MediaPlayerState => mediaControllerCompat?.PlaybackState?.State ?? PlaybackStateCompat.StateNone;

        public MediaSessionCompat CurrentSession => mediaSessionCompat;

        internal Context ApplicationContext => applicationContext;

        internal ComponentName RemoteComponentName { get; set; }

        private readonly IMediaManager mediaManager;
        private IMediaQueue mediaQueue => mediaManager.MediaQueue;

        public MediaSessionManager(Context appContext, IMediaManager mediaManager)
        {
            applicationContext = appContext;
            this.mediaManager = mediaManager;
            UpdateAndroidNotificationManagerSettings();
        }

        internal void InitMediaSession(string packageName, MediaServiceBinder binder)
        {
            try
            {
                if (mediaSessionCompat == null)
                {
                    Intent nIntent = new Intent(applicationContext, typeof(MediaPlayer));
                    PendingIntent pIntent = PendingIntent.GetActivity(applicationContext, 0, nIntent, 0);

                    RemoteComponentName = new ComponentName(packageName, new RemoteControlBroadcastReceiver().ComponentName);
                    mediaSessionCompat = new MediaSessionCompat(applicationContext, "XamarinStreamingAudio", RemoteComponentName, pIntent);

                    mediaControllerCompat = new MediaControllerCompat(applicationContext, mediaSessionCompat.SessionToken);
                    _notificationManager = _overrideNotificationManager ?? new MediaNotificationManagerImplementation(applicationContext, typeof(MediaPlayerService));
                    UpdateAndroidNotificationManagerSettings();
                }
                mediaSessionCompat.Active = true;
                MediaServiceBase mediaServiceBase = binder.GetMediaPlayerService<MediaServiceBase>();
                MediaSessionCompat.Callback remoteCallback = mediaServiceBase.AlternateRemoteCallback;
                if (remoteCallback == null)
                    remoteCallback = new MediaSessionCallback(this);
                try
                {
                    mediaSessionCompat.SetCallback(remoteCallback);
                }
                catch (Exception ex2)
                {
                    Console.WriteLine(ex2);
                }
                mediaSessionCompat?.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons | MediaSessionCompat.FlagHandlesTransportControls);
                _packageName = packageName;
                _binder = binder;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal void Release()
        {
            try
            {
                mediaSessionCompat?.Release();
                mediaSessionCompat?.Dispose();
                mediaSessionCompat = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal void HandleAction(string action)
        {
            OnNotificationActionFired?.Invoke(CurrentSession, action);
        }

        /// <summary>
        /// Updates the state of the player.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="position"></param>
        public void UpdatePlaybackState(int state, int position = 0, string errorMessage = "")
        {
            if (CurrentSession == null && (_binder?.IsBinderAlive).GetValueOrDefault(false) && !string.IsNullOrWhiteSpace(_packageName))
                InitMediaSession(_packageName, _binder);

            PlaybackStateCompat.Builder stateBuilder = new PlaybackStateCompat.Builder()
                    .SetActions(PlaybackStateCompat.ActionPlay
                        | PlaybackStateCompat.ActionPlayPause
                        | PlaybackStateCompat.ActionPause
                        | PlaybackStateCompat.ActionSkipToNext
                        | PlaybackStateCompat.ActionSkipToPrevious
                        | PlaybackStateCompat.ActionStop);

            stateBuilder.SetState(state, position, 0, SystemClock.ElapsedRealtime());
            if (state == PlaybackStateCompat.StateError)
                stateBuilder.SetErrorMessage(errorMessage);
            PlaybackStateCompat playbackStateCompat = stateBuilder.Build();
            try
            {
                CurrentSession?.SetPlaybackState(playbackStateCompat);
            }
            catch (ArgumentException)
            {
                if (state != PlaybackStateCompat.StateError)
                    throw;
            }
            OnStatusChanged?.Invoke(CurrentSession, state);

            //Used for backwards compatibility
            if ((Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                && (CurrentSession?.RemoteControlClient == null
                || (bool)!CurrentSession?.RemoteControlClient.Equals(typeof(RemoteControlClient)))) return;

            RemoteControlClient remoteControlClient = (RemoteControlClient)CurrentSession?.RemoteControlClient;

            RemoteControlFlags flags = RemoteControlFlags.Play
                                       | RemoteControlFlags.Pause
                                       | RemoteControlFlags.PlayPause
                                       | RemoteControlFlags.FastForward;

            remoteControlClient?.SetTransportControlFlags(flags);
        }

        /// <summary>
        /// Updates the metadata on the lock screen
        /// </summary>
        /// <param name="currentTrack"></param>
        internal void UpdateMetadata(IMediaFile currentTrack)
        {
            MediaMetadataCompat.Builder builder = new MediaMetadataCompat.Builder();

            if (currentTrack != null)
            {
                builder
                    .PutString(MediaMetadata.MetadataKeyAlbum, currentTrack.Metadata.Album)
                    .PutString(MediaMetadata.MetadataKeyArtist, currentTrack.Metadata.Artist)
                    .PutString(MediaMetadata.MetadataKeyTitle, currentTrack.Metadata.Title);
            }
            else
            {
                builder
                    .PutString(MediaMetadata.MetadataKeyAlbum,
                        CurrentSession?.Controller?.Metadata?.GetString(MediaMetadata.MetadataKeyAlbum))
                    .PutString(MediaMetadata.MetadataKeyArtist,
                        CurrentSession?.Controller?.Metadata?.GetString(MediaMetadata.MetadataKeyArtist))
                    .PutString(MediaMetadata.MetadataKeyTitle,
                        CurrentSession?.Controller?.Metadata?.GetString(MediaMetadata.MetadataKeyTitle));
            }

            builder.PutBitmap(MediaMetadata.MetadataKeyAlbumArt, currentTrack?.Metadata.AlbumArt as Bitmap);
            CurrentSession?.SetMetadata(builder.Build());
        }
    }
}