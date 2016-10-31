using Android.App;
using Android.OS;
using MediaManager.Sample.Core;
using Android.Support.V7.App;
using Android.Content.PM;
using Android.Widget;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Android.Graphics;
using Plugin.MediaManager;
using Android.Media;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions.Implementations;
using Plugin.MediaManager.ExoPlayer;

namespace MediaSample.Droid
{
    [Activity(Label = "MediaSample.Droid", 
              MainLauncher = true, 
              Icon = "@drawable/icon_play", 
              Theme="@style/AppTheme",
              LaunchMode = LaunchMode.SingleTop,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : AppCompatActivity
    {
        private MediaPlayerViewModel ViewModel { get; set; }

        private Android.Support.V7.Widget.Toolbar toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ViewModel = new MediaPlayerViewModel();
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
            }

            //CrossMediaManager.Current.AudioPlayer = new ExoPlayerAudioImplementation(((MediaManagerImplementation)CrossMediaManager.Current).MediaSessionManager);

            var previous = FindViewById<ImageButton>(Resource.Id.btnPrevious);
            previous.Click += async (sender, args) =>
            {
                await ViewModel.MediaPlayer.PlayPrevious();
            };

            var playpause = FindViewById<ToggleButton>(Resource.Id.btnPlayPause);
            playpause.Click += async (sender, args) =>
            {
                await ViewModel.MediaPlayer.PlayPause();
            };

            var next = FindViewById<ImageButton>(Resource.Id.btnNext);
            next.Click += async (sender, args) =>
            {
                await ViewModel.MediaPlayer.PlayNext();
            };

            var position = FindViewById<TextView>(Resource.Id.textview_position);
            var duration = FindViewById<TextView>(Resource.Id.textview_duration);
            var seekbar = FindViewById<SeekBar>(Resource.Id.player_seekbar);

            seekbar.ProgressChanged += (sender, args) =>
            {
                if (args.FromUser)
                {
                    ViewModel.MediaPlayer.Seek(TimeSpan.FromSeconds(args.Progress));
                }
            };

            ViewModel.MediaPlayer.PlayingChanged += (sender, e) =>
            {
                Console.WriteLine($"Playing changed: Progress => {e.Progress}%");
                RunOnUiThread(() =>
                {
                    seekbar.Max = Convert.ToInt32(e.Duration.TotalSeconds);
                    seekbar.Progress = Convert.ToInt32(Math.Floor(e.Position.TotalSeconds));

                    position.Text = $"{e.Position.Minutes:00}:{e.Position.Seconds:00}";
                    duration.Text = $"{e.Duration.Minutes:00}:{e.Duration.Seconds:00}";
                });
            };

            ViewModel.MediaPlayer.BufferingChanged += (sender, args) =>
            {
                RunOnUiThread(() =>
                {
                    Console.WriteLine($"BufferingChanged: {args.BufferProgress}");
                    seekbar.SecondaryProgress = Convert.ToInt32(args.BufferedTime.TotalSeconds);
                });
            };


            ViewModel.MediaPlayer.StatusChanged += (sender, e) =>
            {
                Console.WriteLine($"StausChanged {e.Status}");
                RunOnUiThread(() =>
                {
                    playpause.Checked = e.Status == MediaPlayerStatus.Playing || e.Status == MediaPlayerStatus.Loading || e.Status == MediaPlayerStatus.Buffering;
                });
            };

            ViewModel.MediaPlayer.MediaFailed += (sender, e) =>
            {
                Console.WriteLine($"Media failed: Message => {e.Exception.Message}");
            };

            var title = FindViewById<TextView>(Resource.Id.textview_title);
            var subtitle = FindViewById<TextView>(Resource.Id.textview_subtitle);
            ViewModel.MediaPlayer.MediaFileChanged += (sender, args) =>
            {
                Console.WriteLine($"File changed: {args.File.Metadata.Title}"); ;
                RunOnUiThread(() =>
                {
                    title.Text = args.File.Metadata.Title;
                    subtitle.Text = args.File.Metadata.Artist;
                    var cover = FindViewById<ImageView>(Resource.Id.imageview_cover);
                    cover.SetImageBitmap(args.File.Metadata.Cover as Bitmap);
                });
            };

            ViewModel.Queue.Add(new MediaFile() { Type = MediaFileType.AudioUrl, Url = "https://ia800806.us.archive.org/15/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3" });
            ViewModel.Queue.Add(new MediaFile() { Type = MediaFileType.AudioUrl, Url = "http://www.bensound.org/bensound-music/bensound-goinghigher.mp3" });
            ViewModel.Queue.Add(new MediaFile() { Type = MediaFileType.AudioUrl, Url = "http://www.montemagno.com/sample.mp3" });
            ViewModel.Queue.Add(new MediaFile() { Type = MediaFileType.AudioUrl, Url = "http://www.bensound.org/bensound-music/bensound-tenderness.mp3" });
        }
    }
}


