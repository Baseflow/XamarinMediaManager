using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.MediaPlayerService;

namespace Plugin.MediaManager.MediaSession
{
    public class MediaSessionManagerImplementation
    {
        private Context applicationContext;
        private MediaControllerCompat mediaControllerCompat;
        private MediaSessionCompat mediaSessionCompat;
        private MediaPlayerServiceBinder _binder;
        private string _packageName;
        
        public IMediaNotificationManager NotificationManager { get; internal set; }

        internal event EventHandler<string> OnNotificationActionFired;
        internal event EventHandler<int> OnStatusChanged;

        internal int MediaPlayerState => mediaControllerCompat?.PlaybackState?.State ?? PlaybackStateCompat.StateNone;

        internal MediaSessionCompat CurrentSession => mediaSessionCompat;

        internal Context ApplicationContext => applicationContext;

        internal ComponentName RemoteComponentName { get; set; }
      
        internal MediaSessionManagerImplementation(Context appContext)
        {
            applicationContext = appContext;
        }

        internal void InitMediaSession(string packageName, MediaPlayerServiceBinder binder)
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
                }
                mediaSessionCompat.Active = true;
                //mediaSessionCompat.SetCallback(binder.GetMediaPlayerService().AlternateRemoteCallback ?? new MediaSessionCallback(binder));
                mediaSessionCompat.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons | MediaSessionCompat.FlagHandlesTransportControls);
                NotificationManager = new MediaNotificationManager(applicationContext, CurrentSession.SessionToken);
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
                mediaSessionCompat.Release();
                mediaSessionCompat.Dispose();
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
        internal void UpdatePlaybackState(int state, int position = 0)
        {
            if(CurrentSession == null && (_binder?.IsBinderAlive).GetValueOrDefault(false) && !string.IsNullOrWhiteSpace(_packageName))
                InitMediaSession(_packageName, _binder);

            PlaybackStateCompat.Builder stateBuilder = new PlaybackStateCompat.Builder()
                    .SetActions(PlaybackStateCompat.ActionPlay
                        | PlaybackStateCompat.ActionPlayPause
                        | PlaybackStateCompat.ActionPause
                        | PlaybackStateCompat.ActionSkipToNext
                        | PlaybackStateCompat.ActionSkipToPrevious
                        | PlaybackStateCompat.ActionStop);

            stateBuilder.SetState(state, position, 0, SystemClock.ElapsedRealtime());
            CurrentSession?.SetPlaybackState(stateBuilder.Build());
            OnStatusChanged?.Invoke(CurrentSession, state);

            //Used for backwards compatibility
            if ((Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                && (CurrentSession?.RemoteControlClient == null
                || (bool) !CurrentSession?.RemoteControlClient.Equals(typeof(RemoteControlClient)))) return;

            RemoteControlClient remoteControlClient = (RemoteControlClient)CurrentSession?.RemoteControlClient;

            RemoteControlFlags flags = RemoteControlFlags.Play
                                       | RemoteControlFlags.Pause
                                       | RemoteControlFlags.PlayPause;

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
                    .PutString(MediaMetadata.MetadataKeyAlbum, currentTrack.Metadata.Artist)
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

            builder.PutBitmap(MediaMetadata.MetadataKeyAlbumArt, currentTrack?.Metadata.Cover as Bitmap);
            CurrentSession?.SetMetadata(builder.Build());
        }
    }
}