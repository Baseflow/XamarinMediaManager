using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using Com.Google.Android.Exoplayer2;
using ElementPlayer.Core;
using Java.Lang;
using Java.Util.Concurrent;
using MediaManager;
using MediaManager.Abstractions.Enums;
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
        const string TAG = "XamarinMediaManagerSample";
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
                if (CrossMediaManager.Current.State == MediaPlayerState.Stopped || CrossMediaManager.Current.State == MediaPlayerState.Failed)
                {
                    /*var item1 = await CrossMediaManager.Current.MediaExtractor.CreateMediaItem("https://ia800806.us.archive.org/15/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3");
                    var item2 = await CrossMediaManager.Current.MediaExtractor.CreateMediaItem("https://ia800605.us.archive.org/32/items/Mp3Playlist_555/CelineDion-IfICould.mp3");
                    var item3 = await CrossMediaManager.Current.MediaExtractor.CreateMediaItem("https://ia800605.us.archive.org/32/items/Mp3Playlist_555/Daughtry-Homeacoustic.mp3");
                    
                    var queue = new List<IMediaItem>() { item1, item2, item3 };*/

                    var queue = new List<string>() {
                        "https://ia800806.us.archive.org/15/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3",
                        "https://ia800605.us.archive.org/32/items/Mp3Playlist_555/CelineDion-IfICould.mp3",
                        "https://ia800605.us.archive.org/32/items/Mp3Playlist_555/Daughtry-Homeacoustic.mp3",
                        "https://storage.googleapis.com/uamp/The_Kyoto_Connection_-_Wake_Up/01_-_Intro_-_The_Way_Of_Waking_Up_feat_Alan_Watts.mp3"
                    };

                    await CrossMediaManager.Current.Play(queue);
                }
                else
                {
                    await CrossMediaManager.Current.PlayPause();
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
                CrossMediaManager.Current.PlayingChanged -= Current_PlayingChanged;
            };

            sbProgress.StopTrackingTouch += async (sender, args) =>
            {
                await CrossMediaManager.Current.SeekTo(TimeSpan.FromMilliseconds(args.SeekBar.Progress));
                CrossMediaManager.Current.PlayingChanged += Current_PlayingChanged;
            };

            CrossMediaManager.Current.SetContext(this);

            CrossMediaManager.Current.BufferingChanged += (object s, MediaManager.Abstractions.EventArguments.BufferingChangedEventArgs e) =>
            {
                RunOnUiThread(() =>
                {
                    Log.Debug(TAG, string.Format("{0:0.##}% Total buffered time is {1:mm\\:ss}", e.BufferProgress, e.BufferedTime));
                });
            };

            /*CrossMediaManager.Current.StatusChanged += (object s, MediaManager.Abstractions.EventArguments.StatusChangedEventArgs e) =>
            {

            };*/

            CrossMediaManager.Current.PlayingChanged += Current_PlayingChanged;
        }

        private void Current_PlayingChanged(object sender, MediaManager.Abstractions.EventArguments.PlayingChangedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                string text = string.Format("{0:0.##}% Total played is {1:mm\\:ss} of {2:mm\\:ss}", e.Progress, e.Position, e.Duration);

                tvPlaying.Text = text;
                Log.Debug(TAG, text);

                sbProgress.Max = Convert.ToInt32(e.Duration.TotalMilliseconds);
                sbProgress.Progress = Convert.ToInt32(System.Math.Floor(e.Position.TotalMilliseconds));
            });
        }
    }
}
