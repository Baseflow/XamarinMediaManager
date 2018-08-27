using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Com.Google.Android.Exoplayer2;
using ElementPlayer.Core;
using Java.Lang;
using Java.Util.Concurrent;
using MediaManager;
using MediaManager.Media;
using MediaManager.Platforms.Android;

namespace ElementPlayer.Android
{
    [Activity(Label = "@string/ApplicationName",
        MainLauncher = true,
        Icon = "@drawable/btn_play_active",
        RoundIcon = "@drawable/btn_play_active",
        Theme = "@style/AppTheme",
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : AppCompatActivity
    {
        private IScheduledExecutorService _executorService = Executors.NewSingleThreadScheduledExecutor();
        private IScheduledFuture _scheduledFuture;
        PlayerViewModel playerViewModel = new PlayerViewModel();
        SeekBar sbProgress;
        TextView tvPlaying;
        Handler handler = new Handler();
        //IMediaItem item;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            FindViewById<ToggleButton>(Resource.Id.btnPlayPause).Click += async (object sender, EventArgs e) =>
            {
                if (CrossMediaManager.Current.Status == MediaPlayerStatus.Stopped || CrossMediaManager.Current.Status == MediaPlayerStatus.Failed)
                {
                    var item1 = await CrossMediaManager.Current.MediaExtractor.CreateMediaItem("https://ia800806.us.archive.org/15/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3");
                    var item2 = await CrossMediaManager.Current.MediaExtractor.CreateMediaItem("https://ia800605.us.archive.org/32/items/Mp3Playlist_555/CelineDion-IfICould.mp3");
                    var item3 = await CrossMediaManager.Current.MediaExtractor.CreateMediaItem("https://ia800605.us.archive.org/32/items/Mp3Playlist_555/Daughtry-Homeacoustic.mp3");

                    var queue = new List<IMediaItem>() { item1, item2, item3 };                    

                    await CrossMediaManager.Current.Play(queue);
                    ScheduleSeekbarUpdate();
                }
                else
                {
                    await CrossMediaManager.Current.PlayPause();

                    if (CrossMediaManager.Current.Status == MediaPlayerStatus.Paused)
                        StopSeekbarUpdate();
                    else
                        ScheduleSeekbarUpdate();
                }
            };

            FindViewById<Button>(Resource.Id.btnNext).Click += async (sender, args) =>
            {
                await CrossMediaManager.Current.PlayNext();
            };

            FindViewById<Button>(Resource.Id.btnPrevious).Click += async (sender, args) =>
            {
                await CrossMediaManager.Current.PlayPrevious();
            };

            sbProgress = FindViewById<SeekBar>(Resource.Id.sbProgress);
            tvPlaying = FindViewById<TextView>(Resource.Id.tvPlaying);

            sbProgress.StartTrackingTouch += async (sender, args) =>
            {
                StopSeekbarUpdate();
            };

            sbProgress.StopTrackingTouch += async (sender, args) =>
            {
                await CrossMediaManager.Current.SeekTo(TimeSpan.FromSeconds(args.SeekBar.Progress));
                ScheduleSeekbarUpdate();
            };

            CrossMediaManager.Current.SetContext(this);
        }

        private void OnPlaying()
        {
            TimeSpan duration = CrossMediaManager.Current.Duration;
            TimeSpan position = CrossMediaManager.Current.Position;

            sbProgress.Max = Convert.ToInt32(duration.TotalMilliseconds);
            sbProgress.Progress = Convert.ToInt32(System.Math.Floor(position.TotalMilliseconds));

            //is {(position.TotalMilliseconds / duration.TotalMilliseconds) * 100: 000}%

            string text = $"{position.Minutes:00}:{position.Seconds:00} of {duration.Minutes:00}:{duration.Seconds:00}";

            tvPlaying.Text = text;
        }

        private void ScheduleSeekbarUpdate()
        {
            StopSeekbarUpdate();
            if (!_executorService.IsShutdown)
            {
                var runnable = new Runnable(() => { handler.Post(OnPlaying); });
                _scheduledFuture = _executorService.ScheduleAtFixedRate(runnable, 100, 1000, TimeUnit.Milliseconds);
            }
        }

        private void StopSeekbarUpdate()
        {
            if (_scheduledFuture != null)
            {
                _scheduledFuture.Cancel(false);
            }
        }
    }
}
