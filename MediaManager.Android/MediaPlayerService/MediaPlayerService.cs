using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager
{
    [Service]
    [IntentFilter(new[] { ActionPlay, ActionPause, ActionStop, ActionTogglePlayback, ActionNext, ActionPrevious })]
    public class MediaPlayerService : MediaServiceBase,
        MediaPlayer.IOnBufferingUpdateListener,
        MediaPlayer.IOnCompletionListener,
        MediaPlayer.IOnErrorListener,
        MediaPlayer.IOnPreparedListener,
        MediaPlayer.IOnSeekCompleteListener
    {
        private MediaPlayer _mediaPlayer;
        private TimeSpan _buffered = TimeSpan.Zero;

        public override TimeSpan Position
        {
            get
            {
                if (_mediaPlayer == null
                    || (MediaPlayerState != PlaybackStateCompat.StatePlaying
                        && MediaPlayerState != PlaybackStateCompat.StatePaused))
                    return TimeSpan.FromSeconds(-1);
                else
                    return TimeSpan.FromMilliseconds(_mediaPlayer.CurrentPosition);
            }
        }

        public override TimeSpan Duration
        {
            get
            {
                if (_mediaPlayer == null
                    || (MediaPlayerState != PlaybackStateCompat.StatePlaying
                        && MediaPlayerState != PlaybackStateCompat.StatePaused))
                    return TimeSpan.Zero;
                else
                    return TimeSpan.FromMilliseconds(_mediaPlayer.Duration);
            }
        }

        public override TimeSpan Buffered
        {
            get
            {
                if (_mediaPlayer == null)
                    return TimeSpan.Zero;
                else
                    return _buffered;
            }
        }

        public override void InitializePlayer()
        {
            DisposeMediaPlayer();
            _mediaPlayer = new MediaPlayer();
            SetMediaPlayerOptions();
        }

        public override void InitializePlayerWithUrl(string audioUrl)
        {
            Android.Net.Uri uri = Android.Net.Uri.Parse(Android.Net.Uri.Encode(audioUrl));
            _mediaPlayer = MediaPlayer.Create(ApplicationContext, uri);
            SetMediaPlayerOptions();
        }

        public override void SetMediaPlayerOptions()
        {
            //Tell our player to sream music
            _mediaPlayer.SetAudioStreamType(Stream.Music);

            //Wake mode will be partial to keep the CPU still running under lock screen
            _mediaPlayer.SetWakeMode(ApplicationContext, WakeLockFlags.Partial);
            _mediaPlayer.SetOnBufferingUpdateListener(this);
            _mediaPlayer.SetOnCompletionListener(this);
            _mediaPlayer.SetOnErrorListener(this);
            _mediaPlayer.SetOnPreparedListener(this);
        }

        [Obsolete("deprecated")]
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            HandleIntent(intent);
            return StartCommandResult.NotSticky;// base.OnStartCommand(intent, flags, startId);
        }

        public override async Task Play(IMediaFile mediaFile = null)
        {
            await base.Play(mediaFile);

            if (mediaFile != null) {
                try
                {
                    _mediaPlayer.PrepareAsync();
                }
                catch (Java.Lang.IllegalStateException)
                {
                    int retryCount = 0;
                    do
                    {
                        await Task.Delay(250);
                        try
                        {
                            _mediaPlayer.Reset();
                            await SetMediaPlayerDataSource();
                            _mediaPlayer.PrepareAsync();
                        }
                        catch (Java.Lang.IllegalStateException)
                        {
                            retryCount++;
                            continue;
                        }
                        return;
                    } while (retryCount < 10);
                }   
            }
        }

        public override void SetVolume(float leftVolume, float rightVolume)
        {
            _mediaPlayer?.SetVolume(leftVolume, rightVolume);
        }

        public override async Task<bool> SetMediaPlayerDataSource()
        {
            if (CurrentFile == null)
                return false;

            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.Honeycomb)
                    try
                    {
                        await SetMediaPlayerDataSourcePreHoneyComb();
                    }
                    catch (Exception e)
                    {
                        await SetMediaPlayerDataSourcePostHoneyComb();
                    }
                else
                    await SetMediaPlayerDataSourcePostHoneyComb();

            }
            catch (Exception e)
            {
                try
                {
                    await SetMediaPlayerDataSourceUsingFileDescriptor();
                }
                catch (Exception ex)
                {
                    if (_mediaPlayer == null)
                        return false;

                    if (CurrentFile == null)
                        return false;
                    String uri = GetUriFromPath(ApplicationContext, CurrentFile.Url);
                    _mediaPlayer?.Reset();
                    try
                    {
                        await _mediaPlayer?.SetDataSourceAsync(ApplicationContext, Android.Net.Uri.Parse(uri), RequestHeaders);
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                }
            }

            return true;
        }

        public override Task Play(IEnumerable<IMediaFile> mediaFiles)
        {
            throw new NotImplementedException();
        }

        public override async Task Seek(TimeSpan position)
        {
            await Task.Run(() =>
            {
                _mediaPlayer?.SeekTo(Convert.ToInt32(position.TotalMilliseconds));
            });
        }

        public override async Task Pause()
        {
            await Task.Run(() =>
            {
                if (_mediaPlayer == null)
                    return;

                if (_mediaPlayer.IsPlaying)
                    _mediaPlayer.Pause();

                if (!TransientPaused)
                    ManuallyPaused = true;
            });
            await base.Pause();
        }

        public override async Task Stop()
        {
            try
            {
                if (_mediaPlayer == null)
                    return;

                if (_mediaPlayer.IsPlaying)
                {
                    _mediaPlayer.Stop();
                }
                _mediaPlayer.Reset();
            }
            catch (Java.Lang.IllegalStateException ex)
            {
                Console.WriteLine(ex.Message);
            }
            await base.Stop();
        }

        public void OnCompletion(MediaPlayer mp)
        {
            OnMediaFinished(new MediaFinishedEventArgs(CurrentFile));
        }

        public bool OnError(MediaPlayer mp, MediaError what, int extra)
        {
            SessionManager.UpdatePlaybackState(PlaybackStateCompat.StateError, Position.Seconds, Enum.GetName(typeof(MediaError), what));
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
            SessionManager.UpdatePlaybackState(PlaybackStateCompat.StatePlaying, Position.Seconds);
        }

        public void OnBufferingUpdate(MediaPlayer mp, int percent)
        {
            int duration = 0;
            if (MediaPlayerState == PlaybackStateCompat.StatePlaying ||
                MediaPlayerState == PlaybackStateCompat.StatePaused)
                duration = mp.Duration;

            int newBufferedTime = duration * percent / 100;
            if (newBufferedTime != Convert.ToInt32(Buffered.TotalSeconds))
            {
                _buffered = TimeSpan.FromSeconds(newBufferedTime);
            }
        }

        protected override void Resume()
        {
            _mediaPlayer.Start();
        }

        private async Task SetMediaPlayerDataSourcePreHoneyComb()
        {
            _mediaPlayer.Reset();
            await _mediaPlayer.SetDataSourceAsync(CurrentFile.Url);
        }

        private async Task SetMediaPlayerDataSourcePostHoneyComb()
        {
            _mediaPlayer?.Reset();
            Android.Net.Uri uri = Android.Net.Uri.Parse(CurrentFile.Url);
            var dataSourceAsync = _mediaPlayer?.SetDataSourceAsync(ApplicationContext, uri, RequestHeaders);
            if (dataSourceAsync != null)
                await dataSourceAsync;
        }

        private async Task SetMediaPlayerDataSourceUsingFileDescriptor()
        {
            Java.IO.File file = new Java.IO.File(CurrentFile.Url);
            Java.IO.FileInputStream inputStream = new Java.IO.FileInputStream(file);
            _mediaPlayer?.Reset();
            var dataSourceAsync = _mediaPlayer?.SetDataSourceAsync(inputStream.FD);
            if (dataSourceAsync != null)
                await dataSourceAsync;
            inputStream.Close();
        }

        public override void OnDestroy()
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Release();
                _mediaPlayer = null;
            }

            base.OnDestroy();
        }

        private void DisposeMediaPlayer()
        {
            _mediaPlayer?.Reset();
            _mediaPlayer?.Release();
            _mediaPlayer?.Dispose();
            _mediaPlayer = null;
        }
    }

}