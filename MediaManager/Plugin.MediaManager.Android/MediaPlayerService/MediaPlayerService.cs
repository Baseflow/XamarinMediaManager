using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Database;
using Android.Media;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Provider;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.MediaSession;

namespace Plugin.MediaManager.MediaPlayerService
{
    [Service]
    [IntentFilter(new[] {ActionPlay, ActionPause, ActionStop, ActionTogglePlayback, ActionNext, ActionPrevious})]
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

        private TimeSpan buffered = TimeSpan.Zero;
        private IMediaFile currentFile;

        internal const int NotificationId = 1;

        private bool manuallyPaused = false;
        private bool transientPaused = false;
        public MediaPlayer mediaPlayer;
        private AudioManager audioManager;
        private MediaSessionManagerImplementation _sessionManager;

        public int MediaPlayerState => _sessionManager.MediaPlayerState;

        IBinder binder;
        private WifiManager wifiManager;
        private WifiManager.WifiLock wifiLock;

        public event StatusChangedEventHandler StatusChanged;
        public event PlayingChangedEventHandler PlayingChanged;
        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFinishedEventHandler MediaFinished;
        public event MediaFailedEventHandler MediaFailed;
        public event MediaFileChangedEventHandler MediaFileChanged;
        public event MediaFileFailedEventHandler MediaFileFailed;

        private Handler PlayingHandler;
        private Java.Lang.Runnable PlayingHandlerRunnable;

        public MediaSessionCompat.Callback AlternateRemoteCallback { get; set; }

        public MediaPlayerService()
        {

            StatusChanged += (sender, args) =>
            {
                if (MediaPlayerState == PlaybackStateCompat.StatePlaying)
                {
                    PlayingHandler.PostDelayed(PlayingHandlerRunnable, 0);
                }
            };
        }

        /// <summary>
        /// On create simply detect some of our managers
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();
            //Find our audio and notificaton managers
            audioManager = (AudioManager) GetSystemService(AudioService);
            wifiManager = (WifiManager) GetSystemService(WifiService);
        }

        /// <summary>
        /// Intializes the player.
        /// </summary>
        private void InitializePlayer()
        {
            mediaPlayer = new MediaPlayer();
            SetMediaPlayerOptions();
        }

        private void InitializePlayerWithURL(string audioUrl)
        {
            Android.Net.Uri uri = Android.Net.Uri.Parse(Android.Net.Uri.Encode(audioUrl));
            mediaPlayer = MediaPlayer.Create(ApplicationContext, uri);
            SetMediaPlayerOptions();
        }

        private void SetMediaPlayerOptions()
        {
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
            if (MediaPlayerState == PlaybackStateCompat.StatePlaying ||
                MediaPlayerState == PlaybackStateCompat.StatePaused)
                duration = mp.Duration;

            int newBufferedTime = duration*percent/100;
            if (newBufferedTime != Convert.ToInt32(Buffered.TotalSeconds))
            {
                Buffered = TimeSpan.FromSeconds(newBufferedTime);
                BufferingChanged?.Invoke(this, new BufferingChangedEventArgs(percent, TimeSpan.FromMilliseconds(newBufferedTime)));
            }
        }

        public void OnCompletion(MediaPlayer mp)
        {
            MediaFinished?.Invoke(this, new MediaFinishedEventArgs(currentFile));
        }

        public bool OnError(MediaPlayer mp, MediaError what, int extra)
        {
            _sessionManager.UpdatePlaybackState(PlaybackStateCompat.StateError, Position.Seconds);
            Stop();
            return true;
        }

        public void OnSeekComplete(MediaPlayer mp)
        {
            //TODO: Implement buffering on seeking
        }

        public void OnPrepared(MediaPlayer mp)
        {
            mp.Start();
            _sessionManager.UpdatePlaybackState(PlaybackStateCompat.StatePlaying, Position.Seconds);
        }

        public TimeSpan Position
        {
            get
            {
                if (mediaPlayer == null
                    || (MediaPlayerState != PlaybackStateCompat.StatePlaying
                        && MediaPlayerState != PlaybackStateCompat.StatePaused))
                    return TimeSpan.FromSeconds(-1);
                else
                    return TimeSpan.FromMilliseconds(mediaPlayer.CurrentPosition);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                if (mediaPlayer == null
                    || (MediaPlayerState != PlaybackStateCompat.StatePlaying
                        && MediaPlayerState != PlaybackStateCompat.StatePaused))
                    return TimeSpan.Zero;
                else
                    return TimeSpan.FromMilliseconds(mediaPlayer.Duration);
            }
        }

        public TimeSpan Buffered
        {
            get
            {
                if (mediaPlayer == null)
                    return TimeSpan.Zero;
                else
                    return buffered;
            }
            private set { buffered = value; }
        }

        public virtual async Task Play(IMediaFile mediaFile = null)
        {
            if (mediaFile != null && !string.IsNullOrEmpty(mediaFile.Url))
            {
                currentFile = mediaFile;
            }

            if (currentFile == null)
            {
                MediaFileFailed?.Invoke(this, new MediaFileFailedEventArgs(new Exception("No mediafile set"), null));
                return;
            }

            manuallyPaused = false;
            if (mediaPlayer != null && MediaPlayerState == PlaybackStateCompat.StatePaused)
            {
                //We are simply paused so just start again
                mediaPlayer.Start();
                _sessionManager.UpdatePlaybackState(PlaybackStateCompat.StatePlaying, Position.Seconds);
                _sessionManager.UpdateMetadata(mediaFile);
                _sessionManager.NotificationManager.StartNotification(mediaFile);
                //Update the metadata now that we are playing
                return;
            }

            DisposeMediaPlayer();
            InitializePlayer();
            _sessionManager.InitMediaSession(PackageName, binder as MediaPlayerServiceBinder);

            if (mediaPlayer.IsPlaying)
            {
                _sessionManager.UpdatePlaybackState(PlaybackStateCompat.StatePlaying, Position.Seconds);
                return;
            }

            bool DataSourceSet = false;
            try
            {
                await SetMediaPlayerDataSource(ApplicationContext, mediaPlayer, currentFile.Url);
                DataSourceSet = true;
            }
            catch (Exception)
            {
                try
                {
                    InitializePlayerWithURL(currentFile.Url);
                    DataSourceSet = true;
                }
                catch (Exception ex)
                {
                    DataSourceSet = false;
                    MediaFileFailed?.Invoke(this, new MediaFileFailedEventArgs(ex, mediaFile));
                }
            }

            if (DataSourceSet)
            {
                try
                {
                    var focusResult = audioManager.RequestAudioFocus(this, Stream.Music, AudioFocus.Gain);
                    if (focusResult != AudioFocusRequest.Granted)
                    {
                        //could not get audio focus
                        Console.WriteLine("Could not get audio focus");
                    }
                    AquireWifiLock();
                    mediaPlayer.PrepareAsync();
                    _sessionManager.UpdatePlaybackState(PlaybackStateCompat.StateBuffering, Position.Seconds);
                }
                catch (Exception)
                {
                    _sessionManager.UpdatePlaybackState(PlaybackStateCompat.StateStopped, Position.Seconds);
                    DisposeMediaPlayer();
                }
              
            }
        }

        private async Task SetMediaPlayerDataSource(Context context, MediaPlayer mp, String fileInfo)
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.Honeycomb)
                    try
                    {
                        await SetMediaPlayerDataSourcePreHoneyComb(context, mp, fileInfo);
                    }
                    catch (Exception e)
                    {
                        await SetMediaPlayerDataSourcePostHoneyComb(context, mp, fileInfo);
                    }
                else
                    await SetMediaPlayerDataSourcePostHoneyComb(context, mp, fileInfo);

            }
            catch (Exception e)
            {
                try
                {
                    await SetMediaPlayerDataSourceUsingFileDescriptor(context, mp, fileInfo);
                }
                catch (Exception ex)
                {
                    String uri = GetUriFromPath(context, fileInfo);
                    mp.Reset();
                    await mp.SetDataSourceAsync(uri);
                }
            }
        }

        private async Task SetMediaPlayerDataSourcePreHoneyComb(Context context, MediaPlayer mp, string fileInfo)
        {
            mp.Reset();
            await mp.SetDataSourceAsync(fileInfo);
        }

        private async Task SetMediaPlayerDataSourcePostHoneyComb(Context context, MediaPlayer mp, string fileInfo)
        {
            mp.Reset();
            Android.Net.Uri uri = Android.Net.Uri.Parse(Android.Net.Uri.Encode(fileInfo));
            await mp.SetDataSourceAsync(context, uri);
        }

        private async Task SetMediaPlayerDataSourceUsingFileDescriptor(Context context, MediaPlayer mp, string fileInfo)
        {
            Java.IO.File file = new Java.IO.File(fileInfo);
            Java.IO.FileInputStream inputStream = new Java.IO.FileInputStream(file);
            mp.Reset();
            await mp.SetDataSourceAsync(inputStream.FD);
            inputStream.Close();
        }

        internal void SetMediaSession(MediaSessionManagerImplementation sessionManager)
        {
            _sessionManager = sessionManager;
            _sessionManager.RemoteComponentName = new ComponentName(PackageName, new RemoteControlBroadcastReceiver().ComponentName);
        }

        private static string GetUriFromPath(Context context, string path)
        {
            Android.Net.Uri uri = MediaStore.Audio.Media.GetContentUriForPath(path);
            ICursor cursor = context.ContentResolver.Query(uri, null, MediaStore.Audio.Media.InterfaceConsts.Data + "='" + path + "'", null, null);
            bool firstSuccess = cursor.MoveToFirst();
            if (!firstSuccess)
                return path;

            int idColumnIndex = cursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Id);
            long id = cursor.GetLong(idColumnIndex);
            cursor.Close();

            if (!uri.ToString().EndsWith(id.ToString()))
            {
                return uri + "/" + id;
            }
            return uri.ToString();
        }

        public async Task Seek(TimeSpan position)
        {
            await Task.Run(() =>
            {
                if (mediaPlayer != null)
                {
                    mediaPlayer.SeekTo(Convert.ToInt32(position.TotalMilliseconds));
                }
            });
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

                _sessionManager.UpdatePlaybackState(PlaybackStateCompat.StatePaused, Position.Seconds);
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

                _sessionManager.UpdatePlaybackState(PlaybackStateCompat.StateStopped, Position.Seconds);

                try
                {
                    mediaPlayer.Reset();
                }
                catch (Java.Lang.IllegalStateException ex)
                { }

                StopForeground(true);
                ReleaseWifiLock();
                _sessionManager.Release();
            });
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

            _sessionManager.HandleAction(intent.Action);
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
            if (wifiLock == null || !wifiLock.IsHeld)
                return;

            try
            {
                wifiLock.Release();
            }
            catch (Java.Lang.RuntimeException)
            { }

            wifiLock = null;
        }

        public override IBinder OnBind(Intent intent)
        {
            binder = new MediaPlayerServiceBinder(this);
            return binder;
        }

        public override bool OnUnbind(Intent intent)
        {
            _sessionManager.NotificationManager.StopNotifications();
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

                _sessionManager.NotificationManager.StopNotifications();
                StopForeground(true);
                ReleaseWifiLock();
                _sessionManager.Release();
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
                        _sessionManager.UpdatePlaybackState(PlaybackStateCompat.StatePlaying, Position.Seconds);
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
                    //We have lost focus but should still play at a muted 10% volume
                    if (mediaPlayer == null)
                        return;

                    if (mediaPlayer.IsPlaying)
                        mediaPlayer.SetVolume(.1f, .1f);//turn it down!
                    break;

            }
        }

        public Task Play(IEnumerable<IMediaFile> mediaFiles)
        {
            throw new NotImplementedException();
        }

        private void DisposeMediaPlayer()
        {
            mediaPlayer?.Reset();
            mediaPlayer?.Release();
            mediaPlayer?.Dispose();
            mediaPlayer = null;
        }

    }
}

