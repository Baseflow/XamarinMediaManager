using Android.App;
using Android.OS;
using MediaManager.Sample.Core;
using Android.Support.V7.App;
using Android.Content.PM;
using Android.Widget;
using System;
using System.Text.RegularExpressions;
using Android.Graphics;
using Newtonsoft.Json.Serialization;
using Plugin.MediaManager.Abstractions.Implementations;
using Plugin.MediaManager.ExoPlayer;
using Plugin.MediaManager;
using System.Collections.Generic;
using Plugin.MediaManager.Abstractions;

namespace MediaSample.Droid
{

    using Plugin.MediaManager.Abstractions.Enums;
    using Plugin.MediaManager.MediaSession;

    [Activity(Label = "MediaSample.Droid",
         MainLauncher = true,
         Icon = "@drawable/icon_play",
         Theme = "@style/AppTheme",
         LaunchMode = LaunchMode.SingleTop,
         ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : AppCompatActivity
    {
        private MediaPlayerViewModel ViewModel { get; set; }

        private IMediaManager MediaManager => ViewModel.MediaPlayer;

        private IPlaybackController PlaybackController => MediaManager.PlaybackController;

        private Android.Support.V7.Widget.Toolbar toolbar;

        private const bool ShouldUseExoPlayer = true;

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

            if (ShouldUseExoPlayer)
            {
                ((MediaManagerImplementation)CrossMediaManager.Current).MediaSessionManager = new MediaSessionManager(Application.Context, typeof(ExoPlayerAudioService), MediaManager);
                var exoPlayer = new ExoPlayerAudioImplementation(((MediaManagerImplementation)CrossMediaManager.Current).MediaSessionManager);
                CrossMediaManager.Current.AudioPlayer = exoPlayer;
            }

            CrossMediaManager.Current.AudioPlayer.RequestHeaders = new Dictionary<string, string> { { "Test", "1234" } };

            var previous = FindViewById<ImageButton>(Resource.Id.btnPrevious);
            previous.Click += async (sender, args) =>
            {
                await PlaybackController.PlayPreviousOrSeekToStart();
            };

            var playpause = FindViewById<ToggleButton>(Resource.Id.btnPlayPause);
            playpause.Click += async (sender, args) =>
            {
                await PlaybackController.PlayPause();
            };

            var next = FindViewById<ImageButton>(Resource.Id.btnNext);
            next.Click += async (sender, args) =>
            {
                await PlaybackController.PlayNext();
            };

            var position = FindViewById<TextView>(Resource.Id.textview_position);
            var duration = FindViewById<TextView>(Resource.Id.textview_duration);
            var seekbar = FindViewById<SeekBar>(Resource.Id.player_seekbar);

            seekbar.ProgressChanged += (sender, args) =>
            {
                if (args.FromUser)
                {
                    PlaybackController.SeekTo(args.Progress);
                }
            };

            ViewModel.MediaPlayer.PlayingChanged += (sender, e) =>

            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        seekbar.Max = Convert.ToInt32(e.Duration.TotalSeconds);
                        seekbar.Progress = Convert.ToInt32(Math.Floor(e.Position.TotalSeconds));

                        position.Text = $"{e.Position.Minutes:00}:{e.Position.Seconds:00}";
                        duration.Text = $"{e.Duration.Minutes:00}:{e.Duration.Seconds:00}";

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception : PlayingChanged: {ex.Message}");
                    }
                });
            };

            ViewModel.MediaPlayer.BufferingChanged += (sender, args) =>
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        Console.WriteLine($"BufferingChanged: {args.BufferedTime.TotalSeconds}");
                        seekbar.SecondaryProgress = Convert.ToInt32(args.BufferedTime.TotalSeconds);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception : BufferingChanged: {ex.Message}");
                    }
                });
            };


            ViewModel.MediaPlayer.StatusChanged += (sender, e) =>
            {
                Console.WriteLine($"StausChanged {e.Status}");
                RunOnUiThread(() =>
                {
                    try
                    {
                        playpause.Checked = e.Status == MediaPlayerStatus.Playing || e.Status == MediaPlayerStatus.Loading ||
                        e.Status == MediaPlayerStatus.Buffering;
                        if (e.Status == MediaPlayerStatus.Stopped || e.Status == MediaPlayerStatus.Failed || e.Status == MediaPlayerStatus.Loading)
                        {
                            seekbar.Progress = 0;
                            seekbar.SecondaryProgress = 0;
                            position.Text = "00:00";
                            duration.Text = "00:00";
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine($"Exception : StausChanged: {ex.Message}");
                    }

                });
            };

            ViewModel.MediaPlayer.MediaFailed += (sender, e) =>
            {
                try
                {
                    Console.WriteLine($"Media failed => {e.Description}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception : MediaFailed: {ex.Message}");
                }
            };

            ViewModel.MediaPlayer.MediaFileFailed += (sender, e) =>
            {
                try
                {
                    Console.WriteLine($"Media file ({e.File.Url}) failed: Message => {e.MediaExeption.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception : MediaFileFailed: {ex.Message}");
                }
            };

            var title = FindViewById<TextView>(Resource.Id.textview_title);
            var subtitle = FindViewById<TextView>(Resource.Id.textview_subtitle);
            ViewModel.MediaPlayer.MediaFileChanged += (sender, args) =>
            {
                try
                {
                    Console.WriteLine($"File changed: {args.File.Metadata.Title}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception : MediaFileChanged: {ex.Message}");
                }

                RunOnUiThread(() =>
                {
                    try
                    {
                        if (args.File.Url == ViewModel.Queue.Current.Url)
                        {
                            title.Text = args.File.Metadata.Title;
                            subtitle.Text = args.File.Metadata.Artist;
                            var cover = FindViewById<ImageView>(Resource.Id.imageview_cover);
                            cover.SetImageBitmap(args.File.Metadata.AlbumArt as Bitmap);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception : MediaFileChangedUI: {ex.Message}");
                    }
                });
            };

            ViewModel.Init();
        }

    }
    public class UnderscoreMappingResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return Regex.Replace(
                propertyName, @"([A-Z])([A-Z][a-z])|([a-z0-9])([A-Z])", "$1$3_$2$4").ToLower();
        }
    }
}


