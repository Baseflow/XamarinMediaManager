using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ElementPlayer.Core;
using Java.Lang;
using Java.Util.Concurrent;
using MediaManager;
using MediaManager.Media;

namespace ElementPlayer.Android
{
    [Activity(Label = "@string/ApplicationName",
        MainLauncher = true,
        Icon = "@drawable/btn_play_active",
        RoundIcon = "@drawable/btn_play_active",
        Theme = "@style/AppTheme",
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : AppCompatActivity//, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private IScheduledExecutorService _executorService = Executors.NewSingleThreadScheduledExecutor();
        private IScheduledFuture _scheduledFuture;
        PlayerViewModel player = new PlayerViewModel();
        SeekBar sbProgress;
        TextView tvPlaying;
        Handler handler = new Handler();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);


            var runnable = new Runnable(() => { handler.Post(OnPlaying); });

            FindViewById<ToggleButton>(Resource.Id.btnPlayPause).Click += async (object sender, System.EventArgs e) =>
            {
                if (player.Status == MediaPlayerStatus.stopped || player.Status == MediaPlayerStatus.failed)
                {
                    await player.Play(await player.CreateMediaItem("https://ia800806.us.archive.org/15/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3"));
                    ScheduleSeekbarUpdate();
                }
                else
                {
                    await player.PlayPause();

                    if (player.Status == MediaPlayerStatus.paused)
                        StopSeekbarUpdate();
                    else
                        ScheduleSeekbarUpdate();
                }
            };

            FindViewById<Button>(Resource.Id.btnNext).Click += async (sender, args) =>
            {
                await player.PlayNext();
            };

            FindViewById<Button>(Resource.Id.btnPrevious).Click += async (sender, args) =>
            {
                await player.PlayPrevious();
            };

            sbProgress = FindViewById<SeekBar>(Resource.Id.sbProgress);
            tvPlaying = FindViewById<TextView>(Resource.Id.tvPlaying);

            sbProgress.StartTrackingTouch += async (sender, args) =>
            {
                StopSeekbarUpdate();
            };

            sbProgress.StopTrackingTouch += async (sender, args) =>
            {
                await player.SeekTo(args.SeekBar.Progress);
                ScheduleSeekbarUpdate();
            };
        }

        private void OnPlaying()
        {
            TimeSpan duration = player.Duration;
            TimeSpan position = player.Position;


            sbProgress.Max = Convert.ToInt32(duration.TotalMilliseconds);
            sbProgress.Progress = Convert.ToInt32(System.Math.Floor(position.TotalMilliseconds));

            //is {(position.TotalMilliseconds / duration.TotalMilliseconds) * 100: 000}%

            string text = $"{position.Minutes:00}:{position.Seconds:00} of {duration.Minutes:00}:{duration.Seconds:00}";

            tvPlaying.Text = text;
            Console.WriteLine(text);
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
