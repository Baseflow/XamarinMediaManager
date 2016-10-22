using Android.App;
using Android.OS;
using MediaManager.Sample.Core;
using Android.Support.V7.App;
using Android.Content.PM;
using Android.Widget;
using System;
using Android.Graphics;
using Plugin.MediaManager;
using Android.Media;
using Android.Support.V4.Media.Session;

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
        private MediaPlayerViewModel ViewModel => new MediaPlayerViewModel();

        private Android.Support.V7.Widget.Toolbar toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
            }

            var previous = FindViewById<ImageButton>(Resource.Id.btnPrevious);
            previous.Click += async (sender, args) =>
            {
                await ViewModel.MediaPlayer.PlayPrevious();
            };

            var playpause = FindViewById<Button>(Resource.Id.btnPlayPause);
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
            ViewModel.MediaPlayer.Playing += (sender, e) => 
            {
                RunOnUiThread(() =>
                {
                    seekbar.Max = ViewModel.Duration;
                    seekbar.Progress = ViewModel.Position;

                    position.Text = ViewModel.GetFormattedTime(ViewModel.Position);
                    duration.Text = ViewModel.GetFormattedTime(ViewModel.Duration);
                });
            };

            ViewModel.MediaPlayer.Buffering += (sender, args) => 
            {
                RunOnUiThread(() =>
                {
                    seekbar.SecondaryProgress = ViewModel.MediaPlayer.Buffered;
                });
            };

            ViewModel.MediaPlayer.CoverReloaded += (object sender, EventArgs e) =>
            {
                RunOnUiThread(() =>
                {
                    var cover = FindViewById<ImageView>(Resource.Id.imageview_cover);
                    cover.SetImageBitmap(ViewModel.Cover as Bitmap);
                });
            };

            var title = FindViewById<TextView>(Resource.Id.textview_title);
            var subtitle = FindViewById<TextView>(Resource.Id.textview_subtitle);
            ViewModel.MediaPlayer.StatusChanged += (sender, args) => 
            {
                var mediaPlayer = ((MediaManagerImplementation)ViewModel.MediaPlayer).Binder.GetMediaPlayerService();

                var metadata = mediaPlayer.mediaControllerCompat.Metadata;
                if (metadata != null)
                {
                    RunOnUiThread(() =>
                    {
                        title.Text = metadata.GetString(MediaMetadata.MetadataKeyTitle);
                        subtitle.Text = metadata.GetString(MediaMetadata.MetadataKeyArtist);
                        playpause.Selected = mediaPlayer.mediaControllerCompat.PlaybackState.State == PlaybackStateCompat.StatePlaying;
                    });
                }
            };

            ViewModel.MediaPlayer.Play("http://www.montemagno.com/sample.mp3");
        }
    }
}


