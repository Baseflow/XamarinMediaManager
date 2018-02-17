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
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;
using Plugin.MediaManager.Audio;
using Plugin.MediaManager.MediaSession;

namespace Plugin.MediaManager
{
    public abstract class MediaServiceBase : Service, AudioManager.IOnAudioFocusChangeListener, IPlaybackManager
    {
        //Actions
        public const string ActionPlay = "com.xamarin.action.PLAY";
        public const string ActionPause = "com.xamarin.action.PAUSE";
        public const string ActionStop = "com.xamarin.action.STOP";
        public const string ActionTogglePlayback = "com.xamarin.action.TOGGLEPLAYBACK";
        public const string ActionNext = "com.xamarin.action.NEXT";
        public const string ActionPrevious = "com.xamarin.action.PREVIOUS";

        //internal const int NotificationId = 1;

        private WifiManager wifiManager;
        private WifiManager.WifiLock wifiLock;
        private IntentFilter _intentFilter;
        private AudioPlayerBroadcastReceiver _noisyAudioStreamReceiver;

        public event StatusChangedEventHandler StatusChanged;
        public event PlayingChangedEventHandler PlayingChanged;
        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFinishedEventHandler MediaFinished;
        public event MediaFailedEventHandler MediaFailed;
        public event MediaFileChangedEventHandler MediaFileChanged;
        public event MediaFileFailedEventHandler MediaFileFailed;

        public bool ManuallyPaused { get; set; } = false;

        public bool TransientPaused { get; set; } = false;

        public AudioManager AudioManager { get; set; }

        public IMediaFile CurrentFile { get; set; }

        public MediaSessionManager SessionManager { get; set; }

        public int MediaPlayerState => SessionManager.MediaPlayerState;

        public IBinder Binder { get; set; }

        public MediaSessionCompat.Callback AlternateRemoteCallback { get; set; }

        public abstract TimeSpan Position { get; }

        public abstract TimeSpan Duration { get; }

        public abstract TimeSpan Buffered { get; }

        public Dictionary<string, string> RequestHeaders { get; set; }

        public MediaPlayerStatus Status
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// On create simply detect some of our managers
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();
            //Find our audio and notificaton managers
            AudioManager = (AudioManager)GetSystemService(AudioService);
            wifiManager = (WifiManager)GetSystemService(WifiService);
        }

        /// <summary>
        /// Intializes the player.
        /// </summary>
        public abstract void InitializePlayer();

        public abstract void InitializePlayerWithUrl(string audioUrl);

        public abstract void SetMediaPlayerOptions();

        public virtual async Task Play(IMediaFile mediaFile = null)
        {
            if (!ValidateMediaFile(mediaFile))
                return;

            bool alreadyPlaying = await CheckIfFileAlreadyIsPlaying(mediaFile);

            if (alreadyPlaying)
                return;

            CurrentFile = mediaFile;

            bool dataSourceSet;
            try
            {
                InitializePlayer();

                // Buffering states similar to how iOS has been implemented
                SessionManager.UpdatePlaybackState(PlaybackStateCompat.StateBuffering);

                SessionManager.InitMediaSession(PackageName, Binder as MediaServiceBinder);
                _noisyAudioStreamReceiver = new AudioPlayerBroadcastReceiver(SessionManager);
                _intentFilter = new IntentFilter(AudioManager.ActionAudioBecomingNoisy);
                SessionManager.ApplicationContext.RegisterReceiver(_noisyAudioStreamReceiver, _intentFilter);
                dataSourceSet = await SetMediaPlayerDataSource();
            }
            catch (Exception ex)
            {
                dataSourceSet = false;
                OnMediaFailed(new MediaFailedEventArgs(ex.ToString(), ex));
            }

            if (dataSourceSet)
            {
                try
                {
                    var focusResult = AudioManager.RequestAudioFocus(this, Stream.Music, AudioFocus.LossTransientCanDuck);
                    if (focusResult != AudioFocusRequest.Granted)
                        Console.WriteLine("Could not get audio focus");

                    AquireWifiLock();
                }
                catch (Exception ex)
                {
                    OnMediaFailed(new MediaFailedEventArgs(ex.ToString(), ex));
                }
            }
        }

        public abstract Task Seek(TimeSpan position);

        public virtual Task Pause()
        {
            SessionManager.UpdatePlaybackState(PlaybackStateCompat.StatePaused, Position.Seconds);
            return Task.CompletedTask;
        }

        public virtual async Task Stop()
        {
            await Task.Run(() =>
            {
                SessionManager.UpdatePlaybackState(PlaybackStateCompat.StateStopped, Position.Seconds);
                SessionManager.NotificationManager.StopNotifications();
                StopForeground(true);
                ReleaseWifiLock();
                SessionManager.Release();
            });
        }

        public abstract Task Play(IEnumerable<IMediaFile> mediaFiles);

        public abstract void SetVolume(float leftVolume, float rightVolume);

        public abstract Task<bool> SetMediaPlayerDataSource();

        public void HandleIntent(Intent intent)
        {
            if (intent?.Action == null || SessionManager == null)
                return;

            SessionManager?.HandleAction(intent?.Action);
        }

        /// <summary>
        /// Lock the wifi so we can still stream under lock screen
        /// </summary>
        public void AquireWifiLock()
        {
            if (wifiLock == null)
            {
                wifiLock = wifiManager.CreateWifiLock(WifiMode.Full, "xamarin_wifi_lock");
            }
            if (!wifiLock.IsHeld)
                wifiLock.Acquire();
        }

        /// <summary>
        /// This will release the wifi lock if it is no longer needed
        /// </summary>
        public void ReleaseWifiLock()
        {
            try
            {
                if (wifiLock == null || !wifiLock.IsHeld)
                    return;
                wifiLock.Release();
            }
            catch (Java.Lang.RuntimeException ex)
            {
                Console.WriteLine(ex.Message);
            }

            wifiLock = null;
        }

        public override IBinder OnBind(Intent intent)
        {
            Binder = new MediaServiceBinder(this);
            return Binder;
        }

        public override bool OnUnbind(Intent intent)
        {
            SessionManager.NotificationManager.StopNotifications();
            if (_noisyAudioStreamReceiver != null)
                SessionManager.ApplicationContext.UnregisterReceiver(_noisyAudioStreamReceiver);
            return base.OnUnbind(intent);
        }

        /// <summary>
        /// Properly cleanup of your player by releasing resources
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();
            SessionManager.NotificationManager.StopNotifications();
            StopForeground(true);
            ReleaseWifiLock();
            SessionManager.Release();
        }

        /// <summary>
        /// For a good user experience we should account for when audio focus has changed.
        /// There is only 1 audio output there may be several media services trying to use it so
        /// we should act correctly based on this.  "duck" to be quiet and when we gain go full.
        /// All applications are encouraged to follow this, but are not enforced.
        /// </summary>
        /// <param name="focusChange"></param>
        public async void OnAudioFocusChange(AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case AudioFocus.Gain:
                    if (TransientPaused && !ManuallyPaused)
                    {
                        await Play();
                    }

                    SetVolume(1.0f, 1.0f);//Turn it up!

                    TransientPaused = false;
                    break;
                case AudioFocus.Loss:
                    //We have lost focus stop!
                    await Stop();
                    break;
                case AudioFocus.LossTransient:
                    //We have lost focus for a short time, but likely to resume so pause
                    TransientPaused = true;
                    await Pause();
                    break;
                case AudioFocus.LossTransientCanDuck:
                    //We have lost focus but should still play at a muted 10% volume
                    SetVolume(.1f, .1f);
                    break;

            }
        }

        internal void SetMediaSession(MediaSessionManager sessionManager)
        {
            SessionManager = sessionManager;
            SessionManager.RemoteComponentName = new ComponentName(PackageName, new RemoteControlBroadcastReceiver().ComponentName);
        }

        public static string GetUriFromPath(Context context, string path)
        {
            Android.Net.Uri uri = MediaStore.Audio.Media.GetContentUriForPath(path);
            string[] selection = { path };
            ICursor cursor = context.ContentResolver.Query(uri, null, MediaStore.Audio.Media.InterfaceConsts.Data + "=?", selection, null);
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

        protected virtual void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }

        protected virtual void OnPlayingChanged(PlayingChangedEventArgs e)
        {
            PlayingChanged?.Invoke(this, e);
        }

        protected virtual void OnBufferingChanged(BufferingChangedEventArgs e)
        {
            BufferingChanged?.Invoke(this, e);
        }

        protected virtual void OnMediaFinished(MediaFinishedEventArgs e)
        {
            MediaFinished?.Invoke(this, e);
        }

        protected virtual void OnMediaFailed(MediaFailedEventArgs e)
        {
            MediaFailed?.Invoke(this, e);
        }

        protected virtual void OnMediaFileChanged(MediaFileChangedEventArgs e)
        {
            MediaFileChanged?.Invoke(this, e);
        }

        protected virtual void OnMediaFileFailed(MediaFileFailedEventArgs e)
        {
            MediaFileFailed?.Invoke(this, e);
        }

        protected abstract void Resume();

        /// <summary>
        /// Checks if player just paused.
        /// </summary>
        /// <param name="mediaFile">The media file.</param>
        private async Task<bool> CheckIfFileAlreadyIsPlaying(IMediaFile mediaFile)
        {
            var isNewFile = CurrentFile == null || string.IsNullOrEmpty(mediaFile?.Url)
                                   || mediaFile?.Url != CurrentFile?.Url;

            //New File selected
            if (isNewFile)
                return await Task.FromResult(false);

            //just paused.. restart playback!
            if (MediaPlayerState == PlaybackStateCompat.StatePaused && ManuallyPaused)
            {
                ManuallyPaused = false;
                SessionManager.UpdatePlaybackState(PlaybackStateCompat.StatePlaying, Position.Seconds);
                SessionManager.UpdateMetadata(mediaFile);
                SessionManager.NotificationManager.StartNotification(mediaFile);
                CurrentFile = mediaFile;
                Resume();
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        private bool ValidateMediaFile(IMediaFile mediaFile)
        {
            if (CurrentFile != null || mediaFile != null) return true;
            OnMediaFileFailed(new MediaFileFailedEventArgs(new Exception("No mediafile set"), null));
            return false;
        }
    }
}

