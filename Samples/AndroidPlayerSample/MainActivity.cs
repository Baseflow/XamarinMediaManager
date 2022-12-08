using Android.Media.Midi;
using MediaManager;

namespace AndroidPlayerSample
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            CrossMediaManager.Current.Init(this);
            CrossMediaManager.Current.MediaPlayer.ShowPlaybackControls = true;
            CrossMediaManager.Current.AutoPlay = true;

            var videoView = FindViewById<MediaManager.Platforms.Android.Video.VideoView>(Resource.Id.exoplayerview_activity_video);
            CrossMediaManager.Current.MediaPlayer.VideoView = videoView;

            CrossMediaManager.Current.Play("http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4");
        }
    }
}
