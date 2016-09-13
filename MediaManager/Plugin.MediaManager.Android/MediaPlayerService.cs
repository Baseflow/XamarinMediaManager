using System;
using Android.App;
using Android.Media;
using Android.Support.V4.Media.Session;
using Android.Net.Wifi;
using Android.Content;
using Android.OS;
using Android.Graphics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Support.V4.Media;
using Android.Support.V4.App;
using Android;
using Android.Net;
using Android.Provider;
using Android.Database;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    [Service]
    [IntentFilter(new[] { ActionPlay, ActionPause, ActionStop, ActionTogglePlayback, ActionNext, ActionPrevious })]
    public class MediaPlayerService : Service, AudioManager.IOnAudioFocusChangeListener,
    MediaPlayer.IOnBufferingUpdateListener,
    MediaPlayer.IOnCompletionListener,
    MediaPlayer.IOnErrorListener,
    MediaPlayer.IOnPreparedListener,
    MediaPlayer.IOnSeekCompleteListener
    {
        //Actions
        public const string ActionPlay = "com.xamarin.action.PLAY";
        public const string ActionPause = "com.xamarin.action.PAUSE";
        public const string ActionStop = "com.xamarin.action.STOP";
        public const string ActionTogglePlayback = "com.xamarin.action.TOGGLEPLAYBACK";
        public const string ActionNext = "com.xamarin.action.NEXT";
        public const string ActionPrevious = "com.xamarin.action.PREVIOUS";

        private string audioUrl { get; set; }

        private bool manuallyPaused = false;
        private bool transientPaused = false;

        public IMediaQueue Queue { get; set; } = new MediaQueue();

        public MediaPlayer mediaPlayer;
        private AudioManager audioManager;

        private MediaSessionCompat mediaSessionCompat;
        public MediaControllerCompat mediaControllerCompat;

        public int MediaPlayerState
        {
            get
            {
                if (mediaControllerCompat == null || mediaControllerCompat.PlaybackState == null)
                    return PlaybackStateCompat.StateNone;
                return mediaControllerCompat.PlaybackState.State;
            }
        }

        private PlayerStatus status;

        public PlayerStatus Status
        {
            get
            {
                switch (MediaPlayerState)
                {
                    case PlaybackStateCompat.StateFastForwarding:
                    case PlaybackStateCompat.StateRewinding:
                    case PlaybackStateCompat.StateSkippingToNext:
                    case PlaybackStateCompat.StateSkippingToPrevious:
                    case PlaybackStateCompat.StateSkippingToQueueItem:
                    case PlaybackStateCompat.StatePlaying:
                        return PlayerStatus.PLAYING;

                    case PlaybackStateCompat.StatePaused:
                        return PlayerStatus.PAUSED;

                    case PlaybackStateCompat.StateConnecting:
                    case PlaybackStateCompat.StateBuffering:
                        return PlayerStatus.BUFFERING;

                    case PlaybackStateCompat.StateError:
                    case PlaybackStateCompat.StateStopped:
                        return PlayerStatus.STOPPED;

                    default:
                        return PlayerStatus.STOPPED;
                }
            }
            private set
            {
                status = value;
                OnStatusChanged(EventArgs.Empty);
            }
        }

        private WifiManager wifiManager;
        private WifiManager.WifiLock wifiLock;
        private ComponentName remoteComponentName;

        private const int NotificationId = 1;

        public event StatusChangedEventHandler StatusChanged;

        public event CoverReloadedEventHandler CoverReloaded;

        public event PlayingEventHandler Playing;

        public event BufferingEventHandler Buffering;

        public event TrackFinishedEventHandler TrackFinished;

        private Handler PlayingHandler;
        private Java.Lang.Runnable PlayingHandlerRunnable;

        public MediaSessionCompat.Callback AlternateRemoteCallback { get; set; }

        public MediaPlayerService()
        {
            // Create an instance for a runnable-handler
            PlayingHandler = new Handler();

            // Create a runnable, restarting itself if the status still is "playing"
            PlayingHandlerRunnable = new Java.Lang.Runnable(() =>
            {
                OnPlaying(EventArgs.Empty);

                if (MediaPlayerState == PlaybackStateCompat.StatePlaying)
                {
                    PlayingHandler.PostDelayed(PlayingHandlerRunnable, 250);
                }
            });

            // On Status changed to PLAYING, start raising the Playing event
            StatusChanged += (object sender, EventArgs e) =>
            {
                if (MediaPlayerState == PlaybackStateCompat.StatePlaying)
                {
                    PlayingHandler.PostDelayed(PlayingHandlerRunnable, 0);
                }
            };
        }

        protected virtual void OnStatusChanged(EventArgs e)
        {
            if (StatusChanged != null)
                StatusChanged(this, e);
        }

        protected virtual void OnCoverReloaded(EventArgs e)
        {
            if (CoverReloaded != null)
            {
                CoverReloaded(this, e);
                StartNotification();
                UpdateMediaMetadataCompat();
            }
        }

        protected virtual void OnPlaying(EventArgs e)
        {
            if (Playing != null)
                Playing(this, e);
        }

        protected virtual void OnBuffering(EventArgs e)
        {
            if (Buffering != null)
                Buffering(this, e);
        }

        /// <summary>
        /// On create simply detect some of our managers
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();
            //Find our audio and notificaton managers
            audioManager = (AudioManager)GetSystemService(AudioService);
            wifiManager = (WifiManager)GetSystemService(WifiService);

            remoteComponentName = new ComponentName(PackageName, new RemoteControlBroadcastReceiver().ComponentName);
        }

        /// <summary>
        /// Will register for the remote control client commands in audio manager
        /// </summary>
        private void InitMediaSession()
        {
            try
            {
                if (mediaSessionCompat == null)
                {
                    Intent nIntent = new Intent(ApplicationContext, typeof(MediaPlayer));
                    PendingIntent pIntent = PendingIntent.GetActivity(ApplicationContext, 0, nIntent, 0);

                    remoteComponentName = new ComponentName(PackageName, new RemoteControlBroadcastReceiver().ComponentName);

                    mediaSessionCompat = new MediaSessionCompat(ApplicationContext, "XamarinStreamingAudio", remoteComponentName, pIntent);
                    mediaControllerCompat = new MediaControllerCompat(ApplicationContext, mediaSessionCompat.SessionToken);
                }

                mediaSessionCompat.Active = true;

                if (AlternateRemoteCallback != null)
                    mediaSessionCompat.SetCallback(AlternateRemoteCallback);
                else
                    mediaSessionCompat.SetCallback(new MediaSessionCallback((MediaPlayerServiceBinder)binder));

                mediaSessionCompat.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons | MediaSessionCompat.FlagHandlesTransportControls);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Intializes the player.
        /// </summary>
        private void InitializePlayer()
        {
            mediaPlayer = new MediaPlayer();

            //Tell our player to sream music
            mediaPlayer.SetAudioStreamType(Stream.Music);

            //Wake mode will be partial to keep the CPU still running under lock screen
            mediaPlayer.SetWakeMode(ApplicationContext, WakeLockFlags.Partial);

            mediaPlayer.SetOnBufferingUpdateListener(this);
            mediaPlayer.SetOnCompletionListener(this);
            mediaPlayer.SetOnErrorListener(this);
            mediaPlayer.SetOnPreparedListener(this);
        }


        public void OnBufferingUpdate(MediaPlayer mp, int percent)
        {
            int duration = 0;
            if (MediaPlayerState == PlaybackStateCompat.StatePlaying || MediaPlayerState == PlaybackStateCompat.StatePaused)
                duration = mp.Duration;

            int newBufferedTime = duration * percent / 100;
            if (newBufferedTime != Buffered)
            {
                Buffered = newBufferedTime;
            }
        }

        public async void OnCompletion(MediaPlayer mp)
        {
            await PlayNext ();
            if (TrackFinished != null)
                TrackFinished(this, new EventArgs());
        }

        public bool OnError(MediaPlayer mp, MediaError what, int extra)
        {

            UpdatePlaybackState(PlaybackStateCompat.StateError);
            Stop();
            return true;
        }

        public void OnSeekComplete(MediaPlayer mp)
        {
            //TODO: Implement buffering on seeking
        }

        public void OnPrepared(MediaPlayer mp)
        {
            //Mediaplayer is prepared start track playback
            mp.Start();
            UpdatePlaybackState(PlaybackStateCompat.StatePlaying);
        }

        public int Position
        {
            get
            {
                if (mediaPlayer == null
                    || (MediaPlayerState != PlaybackStateCompat.StatePlaying
                        && MediaPlayerState != PlaybackStateCompat.StatePaused))
                    return -1;
                else
                    return mediaPlayer.CurrentPosition;
            }
        }

        public int Duration
        {
            get
            {
                if (mediaPlayer == null
                    || (MediaPlayerState != PlaybackStateCompat.StatePlaying
                        && MediaPlayerState != PlaybackStateCompat.StatePaused))
                    return 0;
                else
                    return mediaPlayer.Duration;
            }
        }

        private int buffered = 0;

        public int Buffered
        {
            get
            {
                if (mediaPlayer == null)
                    return 0;
                else
                    return buffered;
            }
            private set
            {
                buffered = value;
                OnBuffering(EventArgs.Empty);
            }
        }

        private Bitmap cover;

        public object Cover
        {
            get
            {
                if (cover == null)
                    cover = BitmapFactory.DecodeResource(Resources, Resource.Drawable.ButtonStar);
                return cover;
            }
            private set
            {
                cover = value as Bitmap;
                OnCoverReloaded(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Intializes the player.
        /// </summary>
        public async Task Play(string url)
        {
            if (!string.IsNullOrEmpty(url))
                audioUrl = url;
            await Play();
        }

        public async Task Play()
        {
            manuallyPaused = false;
            if (mediaPlayer != null && MediaPlayerState == PlaybackStateCompat.StatePaused)
            {
                //We are simply paused so just start again
                mediaPlayer.Start();
                UpdatePlaybackState(PlaybackStateCompat.StatePlaying);
                StartNotification();

                //Update the metadata now that we are playing
                UpdateMediaMetadataCompat();
                return;
            }

            if (mediaPlayer != null)
            {
                mediaPlayer.Reset();
                mediaPlayer.Release();
                mediaPlayer = null;
            }

            //if (mediaPlayer == null)
            InitializePlayer();

            //if(mediaSessionCompat == null)
            InitMediaSession();

            if (mediaPlayer.IsPlaying)
            {
                UpdatePlaybackState(PlaybackStateCompat.StatePlaying);
                return;
            }

            try
            {
                MediaMetadataRetriever metaRetriever = new MediaMetadataRetriever();

                await mediaPlayer.SetDataSourceAsync(ApplicationContext, Android.Net.Uri.Parse(audioUrl));

                await metaRetriever.SetDataSourceAsync(audioUrl, new Dictionary<string, string>());

                var focusResult = audioManager.RequestAudioFocus(this, Stream.Music, AudioFocus.Gain);
                if (focusResult != AudioFocusRequest.Granted)
                {
                    //could not get audio focus
                    Console.WriteLine("Could not get audio focus");
                }

                UpdatePlaybackState(PlaybackStateCompat.StateBuffering);
                mediaPlayer.PrepareAsync();

                AquireWifiLock();
                UpdateMediaMetadataCompat(metaRetriever);
                StartNotification();

                byte[] imageByteArray = metaRetriever.GetEmbeddedPicture();
                if (imageByteArray == null)
                {
                    Bitmap coverBitmap = GetCurrentTrackCover();
                    if (coverBitmap != null)
                        Cover = coverBitmap;
                    else
                        Cover = await BitmapFactory.DecodeResourceAsync(Resources, Resource.Drawable.ButtonStar);
                }
                else
                    Cover = await BitmapFactory.DecodeByteArrayAsync(imageByteArray, 0, imageByteArray.Length);
            }
            catch (Exception ex)
            {
                UpdatePlaybackState(PlaybackStateCompat.StateStopped);

                if (mediaPlayer != null)
                {
                    mediaPlayer.Reset();
                    mediaPlayer.Release();
                    mediaPlayer = null;
                }

                //unable to start playback log error
                Console.WriteLine(ex);
            }
        }

        private Bitmap GetCurrentTrackCover()
        {
            string albumFolder = GetCurrentSongFolder();
            if (albumFolder == null)
                return null;

            if (!albumFolder.EndsWith("/"))
            {
                albumFolder += "/";
            }

            System.Uri baseUri = new System.Uri(albumFolder);
            string albumArtPath;
            albumArtPath = TryGetAlbumArtPathByFilename(baseUri, "Folder.jpg");
            if (albumArtPath == null)
            {
                albumArtPath = TryGetAlbumArtPathByFilename(baseUri, "Cover.jpg");
                if (albumArtPath == null)
                {
                    albumArtPath = TryGetAlbumArtPathByFilename(baseUri, "AlbumArtSmall.jpg");
                    if (albumArtPath == null)
                        return null;
                }
            }

            Bitmap bitmap = BitmapFactory.DecodeFile(albumArtPath);

            return bitmap;
        }

        private static string TryGetAlbumArtPathByFilename(System.Uri baseUri, string filename)
        {
            //if (!filename.EndsWith("/"))
            //    filename += "/";

            System.Uri testUri = new System.Uri(baseUri, filename);
            string testPath = testUri.LocalPath;
            if (System.IO.File.Exists(testPath))
                return testPath;
            else
                return null;
        }

        private string GetCurrentSongFolder()
        {
            if (!new System.Uri(audioUrl).IsFile)
                return null;

            return System.IO.Path.GetDirectoryName(audioUrl);



            ////var theUri = MediaStore.Audio.Media.ExternalContentUri;
            //Android.Net.Uri theUri = new Android.Net.Uri.Parse("content://media/external/audio/albumart");
            //string path = null;
            //// The projection contains the columns we want to return in our query.
            //string[] projection = new[] { Android.Provider.MediaStore.Audio.AlbumColumns.AlbumArt };
            //using (ICursor cursor = ContentResolver.Query(theUri, projection, null, null, null))
            ////ManagedQuery(uri, projection, null, null, null))
            //{
            //    if (cursor != null)
            //    {
            //        int columnIndex = cursor.GetColumnIndexOrThrow(projection[0]);
            //        cursor.MoveToFirst();
            //        path = cursor.GetString(columnIndex);
            //    }
            //}
            //return path;
        }

        public async Task Seek(int position)
        {
            await Task.Run(() =>
            {
                if (mediaPlayer != null)
                {
                    mediaPlayer.SeekTo(position);
                }
            });
        }

        public async Task PlayNext()
        {
            if (Queue.HasNext())
            {
                UpdatePlaybackState(PlaybackStateCompat.StateSkippingToNext);

                if (mediaPlayer != null)
                {
                    mediaPlayer.Reset();
                }

                Queue.SetNextAsCurrent();
                await Play();
            }
            else
            {
                // If you don't have a next song in the queue, stop and show the meta-data of the first song.
                UpdatePlaybackState(PlaybackStateCompat.StateStopped);
                mediaPlayer.Reset();

                Queue.SetIndexAsCurrent(0);
            }
        }

        public async Task PlayPrevious()
        {
            // Start current track from beginning if it's the first track or the track has played more than 3sec and you hit "playPrevious".
            if (!Queue.HasPrevious() || Position > 3000)
            {
                await Seek(0);
            }
            else
            {
                UpdatePlaybackState(PlaybackStateCompat.StateSkippingToPrevious);

                if (mediaPlayer != null)
                {
                    mediaPlayer.Reset();
                }

                Queue.SetPreviousAsCurrent();
                await Play();
            }
        }

        public async Task PlayPause()
        {
            if (mediaPlayer == null || (mediaPlayer != null && MediaPlayerState == PlaybackStateCompat.StatePaused))
            {
                await Play();
            }
            else
            {
                await Pause();
            }
        }

        public async Task Pause()
        {
            await Task.Run(() =>
            {
                if (mediaPlayer == null)
                    return;

                if (mediaPlayer.IsPlaying)
                    mediaPlayer.Pause();

                if (!transientPaused)
                    manuallyPaused = true;

                UpdatePlaybackState(PlaybackStateCompat.StatePaused);
            });
        }

        public async Task Stop()
        {
            await Task.Run(() =>
            {
                if (mediaPlayer == null)
                    return;

                if (mediaPlayer.IsPlaying)
                {
                    mediaPlayer.Stop();
                }

                UpdatePlaybackState(PlaybackStateCompat.StateStopped);
                mediaPlayer.Reset();
                StopNotification();
                StopForeground(true);
                ReleaseWifiLock();
                UnregisterMediaSessionCompat();
            });
        }

        private void UpdatePlaybackState(int state)
        {

            if (mediaSessionCompat == null || mediaPlayer == null)
                return;

            try
            {
                PlaybackStateCompat.Builder stateBuilder = new PlaybackStateCompat.Builder()
                    .SetActions(PlaybackStateCompat.ActionPlay
                        | PlaybackStateCompat.ActionPlayPause
                        | PlaybackStateCompat.ActionPause
                        | PlaybackStateCompat.ActionSkipToNext
                        | PlaybackStateCompat.ActionSkipToPrevious
                        | PlaybackStateCompat.ActionStop);

                stateBuilder.SetState(state, mediaPlayer.CurrentPosition, 0, SystemClock.ElapsedRealtime());

                mediaSessionCompat.SetPlaybackState(stateBuilder.Build());

                //Used for backwards compatibility
                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    if (mediaSessionCompat.RemoteControlClient != null && mediaSessionCompat.RemoteControlClient.Equals(typeof(RemoteControlClient)))
                    {
                        RemoteControlClient remoteControlClient = (RemoteControlClient)mediaSessionCompat.RemoteControlClient;

                        RemoteControlFlags flags = RemoteControlFlags.Play
                            | RemoteControlFlags.Pause
                            | RemoteControlFlags.PlayPause;

                        remoteControlClient.SetTransportControlFlags(flags);
                    }
                }

                OnStatusChanged(EventArgs.Empty);

                if (state == PlaybackStateCompat.StatePlaying || state == PlaybackStateCompat.StatePaused)
                {
                    StartNotification();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// When we start on the foreground we will present a notification to the user
        /// When they press the notification it will take them to the main page so they can control the music
        /// </summary>
        private void StartNotification()
        {
            if (mediaSessionCompat == null)
                return;

            var pendingIntent = PendingIntent.GetActivity(ApplicationContext, 0, new Intent(ApplicationContext, typeof(MediaPlayer)), PendingIntentFlags.UpdateCurrent);
            MediaMetadataCompat currentTrack = mediaControllerCompat.Metadata;

            Android.Support.V7.App.NotificationCompat.MediaStyle style = new Android.Support.V7.App.NotificationCompat.MediaStyle();
            style.SetMediaSession(mediaSessionCompat.SessionToken);

            Intent intent = new Intent(ApplicationContext, typeof(MediaPlayerService));
            intent.SetAction(ActionStop);
            PendingIntent pendingCancelIntent = PendingIntent.GetService(ApplicationContext, 1, intent, PendingIntentFlags.CancelCurrent);

            style.SetShowCancelButton(true);
            style.SetCancelButtonIntent(pendingCancelIntent);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(ApplicationContext)
                .SetStyle(style)
                .SetContentTitle(currentTrack.GetString(MediaMetadata.MetadataKeyTitle))
                .SetContentText(currentTrack.GetString(MediaMetadata.MetadataKeyArtist))
                .SetContentInfo(currentTrack.GetString(MediaMetadata.MetadataKeyAlbum))
                .SetSmallIcon(Resource.Drawable.ButtonStar)
                .SetLargeIcon(Cover as Bitmap)
                .SetContentIntent(pendingIntent)
                .SetShowWhen(false)
                .SetOngoing(MediaPlayerState == PlaybackStateCompat.StatePlaying)
                .SetVisibility(NotificationCompat.VisibilityPublic);

            builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaPrevious, "Previous", ActionPrevious));
            AddPlayPauseActionCompat(builder);
            builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaNext, "Next", ActionNext));
            style.SetShowActionsInCompactView(0, 1, 2);

            NotificationManagerCompat.From(ApplicationContext).Notify(NotificationId, builder.Build());
        }

        private NotificationCompat.Action GenerateActionCompat(int icon, String title, String intentAction)
        {
            Intent intent = new Intent(ApplicationContext, typeof(MediaPlayerService));
            intent.SetAction(intentAction);

            PendingIntentFlags flags = PendingIntentFlags.UpdateCurrent;
            if (intentAction.Equals(ActionStop))
                flags = PendingIntentFlags.CancelCurrent;

            PendingIntent pendingIntent = PendingIntent.GetService(ApplicationContext, 1, intent, flags);

            return new NotificationCompat.Action.Builder(icon, title, pendingIntent).Build();
        }

        private void AddPlayPauseActionCompat(NotificationCompat.Builder builder)
        {
            if (MediaPlayerState == PlaybackStateCompat.StatePlaying)
                builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaPause, "Pause", ActionPause));
            else
                builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaPlay, "Play", ActionPlay));
        }

        public void StopNotification()
        {
            NotificationManagerCompat nm = NotificationManagerCompat.From(ApplicationContext);
            nm.CancelAll();
        }

        /// <summary>
        /// Updates the metadata on the lock screen
        /// </summary>
        private void UpdateMediaMetadataCompat(MediaMetadataRetriever metaRetriever = null)
        {
            if (mediaSessionCompat == null)
                return;

            MediaMetadataCompat.Builder builder = new MediaMetadataCompat.Builder();

            if (metaRetriever != null)
            {
                builder
                    .PutString(MediaMetadata.MetadataKeyAlbum, metaRetriever.ExtractMetadata(MetadataKey.Album))
                    .PutString(MediaMetadata.MetadataKeyArtist, metaRetriever.ExtractMetadata(MetadataKey.Artist))
                    .PutString(MediaMetadata.MetadataKeyTitle, metaRetriever.ExtractMetadata(MetadataKey.Title));
            }
            else
            {
                builder
                    .PutString(MediaMetadata.MetadataKeyAlbum, mediaSessionCompat.Controller.Metadata.GetString(MediaMetadata.MetadataKeyAlbum))
                    .PutString(MediaMetadata.MetadataKeyArtist, mediaSessionCompat.Controller.Metadata.GetString(MediaMetadata.MetadataKeyArtist))
                    .PutString(MediaMetadata.MetadataKeyTitle, mediaSessionCompat.Controller.Metadata.GetString(MediaMetadata.MetadataKeyTitle));
            }
            builder.PutBitmap(MediaMetadata.MetadataKeyAlbumArt, Cover as Bitmap);

            mediaSessionCompat.SetMetadata(builder.Build());
        }

        [Obsolete("deprecated")]
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            HandleIntent(intent);
            return base.OnStartCommand(intent, flags, startId);
        }

        private void HandleIntent(Intent intent)
        {
            if (intent == null || intent.Action == null)
                return;

            String action = intent.Action;

            if (action.Equals(ActionPlay))
            {
                mediaControllerCompat.GetTransportControls().Play();
            }
            else if (action.Equals(ActionPause))
            {
                mediaControllerCompat.GetTransportControls().Pause();
            }
            else if (action.Equals(ActionPrevious))
            {
                mediaControllerCompat.GetTransportControls().SkipToPrevious();
            }
            else if (action.Equals(ActionNext))
            {
                mediaControllerCompat.GetTransportControls().SkipToNext();
            }
            else if (action.Equals(ActionStop))
            {
                mediaControllerCompat.GetTransportControls().Stop();
            }
        }

        /// <summary>
        /// Lock the wifi so we can still stream under lock screen
        /// </summary>
        private void AquireWifiLock()
        {
            if (wifiLock == null)
            {
                wifiLock = wifiManager.CreateWifiLock(WifiMode.Full, "xamarin_wifi_lock");
            }
            wifiLock.Acquire();
        }

        /// <summary>
        /// This will release the wifi lock if it is no longer needed
        /// </summary>
        private void ReleaseWifiLock()
        {
            if (wifiLock == null)
                return;

            wifiLock.Release();
            wifiLock = null;
        }

        private void UnregisterMediaSessionCompat()
        {
            try
            {
                mediaSessionCompat.Dispose();
                mediaSessionCompat = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        IBinder binder;

        public override IBinder OnBind(Intent intent)
        {
            binder = new MediaPlayerServiceBinder(this);
            return binder;
        }

        public override bool OnUnbind(Intent intent)
        {
            StopNotification();
            return base.OnUnbind(intent);
        }

        /// <summary>
        /// Properly cleanup of your player by releasing resources
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();
            if (mediaPlayer != null)
            {
                mediaPlayer.Release();
                mediaPlayer = null;

                StopNotification();
                StopForeground(true);
                ReleaseWifiLock();
                UnregisterMediaSessionCompat();
            }
        }

        /// <summary>
        /// For a good user experience we should account for when audio focus has changed.
        /// There is only 1 audio output there may be several media services trying to use it so
        /// we should act correctly based on this.  "duck" to be quiet and when we gain go full.
        /// All applications are encouraged to follow this, but are not enforced.
        /// </summary>
        /// <param name="focusChange"></param>
        public void OnAudioFocusChange(AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case AudioFocus.Gain:
                    if (mediaPlayer == null)
                        InitializePlayer();

                    if (!mediaPlayer.IsPlaying && !manuallyPaused)
                    {
                        mediaPlayer.Start();
                        UpdatePlaybackState(PlaybackStateCompat.StatePlaying);
                    }

                    mediaPlayer.SetVolume(1.0f, 1.0f);//Turn it up!

                    transientPaused = false;
                    break;
                case AudioFocus.Loss:
                    //We have lost focus stop!
                    Stop();
                    break;
                case AudioFocus.LossTransient:
                    //We have lost focus for a short time, but likely to resume so pause
                    transientPaused = true;
                    Pause();
                    break;
                case AudioFocus.LossTransientCanDuck:
                    //We have lost focus but should till play at a muted 10% volume
                    if (mediaPlayer.IsPlaying)
                        mediaPlayer.SetVolume(.1f, .1f);//turn it down!
                    break;

            }
        }

        public class MediaSessionCallback : MediaSessionCompat.Callback
        {

            private MediaPlayerServiceBinder mediaPlayerService;
            public MediaSessionCallback(MediaPlayerServiceBinder service)
            {
                mediaPlayerService = service;
            }

            public override void OnPause()
            {
                mediaPlayerService.GetMediaPlayerService().Pause();
                base.OnPause();
            }

            public override void OnPlay()
            {
                mediaPlayerService.GetMediaPlayerService().Play();
                base.OnPlay();
            }

            public override void OnSkipToNext()
            {
                mediaPlayerService.GetMediaPlayerService().PlayNext();
                base.OnSkipToNext();
            }

            public override void OnSkipToPrevious()
            {
                mediaPlayerService.GetMediaPlayerService().PlayPrevious();
                base.OnSkipToPrevious();
            }

            public override void OnStop()
            {
                mediaPlayerService.GetMediaPlayerService().Stop();
                base.OnStop();
            }
        }
    }
}

