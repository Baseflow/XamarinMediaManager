using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using MediaManager;

namespace AndroidPlayerSample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);


            CrossMediaManager.Android.LoadControlSettings.DefaultBufferForPlaybackMs = 500;
            CrossMediaManager.Android.LoadControlSettings.DefaultBufferForPlaybackMs = 500;
            
            CrossMediaManager.Current.Init(this);
            CrossMediaManager.Current.Notification.ShowNavigationControls = true;
            CrossMediaManager.Current.MediaPlayer.ShowPlaybackControls = true;
            CrossMediaManager.Current.AutoPlay = true;

            //var videoView = FindViewById<MediaManager.Platforms.Android.Video.VideoView>(Resource.Id.exoplayerview_activity_video);
            //CrossMediaManager.Current.MediaPlayer.VideoView = videoView;

            //CrossMediaManager.Current.Play("http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4");

            CrossMediaManager.Current.Play(Mp3UrlList);

            CrossMediaManager.Current.MediaItemFailed += MediaItemFailed;
            CrossMediaManager.Current.Queue.QueueChanged += Queue_QueueChanged;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }



        private void Queue_QueueChanged(object sender, MediaManager.Queue.QueueChangedEventArgs e)
        {
            var textLabel = FindViewById<TextView>(Resource.Id.track_name);
            if (textLabel != null)
            {
                textLabel.Text = e.MediaItem?.DisplayTitle ?? GetFileName(e.MediaItem?.MediaUri);
            }
        }

        private string? GetFileName(string? mediaUri)
        {
            if (mediaUri == null)
            {
                return null;
            }
            Uri uri = new Uri(mediaUri);


            return uri.Segments.LastOrDefault();
        }

        private void MediaItemFailed(object sender, MediaManager.Media.MediaItemFailedEventArgs e)
        {

        }

        public IList<string> Mp3UrlList => new[]{
            "https://www.pcorgan.com/mp3/Kampen%20II%20-%20G.F.Handel,%20Jan%20Mulder%20-%20Hornpipe,%20k%20Wil%20U,%20o%20God,%20mijn%20dank%20betalen.mp3",
            "https://www.pcorgan.com/mp3/KampenIII%20-%20Cor%20van%20Dijk%20-%20Ps%2081v3%20-%20Ped%20Sub16%20Ged8%20Oct8%20Rw-P%20Rw%20Pr8%20Oct4%20GedQ3%20Fl2%20Hw%20Tromp8%20Bv-Hw%20Bv%20Tromp8.mp3",
            "https://www.pcorgan.com/mp3/Kampen3%20-%20Peter%20de%20Wilde%20-%20Ps%20121v1%20-%20A%20Bd16%20Hp8%20Fl4%20B%20Ped%20Sb16%20Gd8%20Hw-P%20Rw%20Hp8%20GdQ3%20Trem%20Hw%20Hp8%20Fl4%20Bw%20Tr8.mp3",
            "https://www.pcorgan.com/mp3/Caen%20-%20Marco%20den%20Toom%20-%20Psalm%20134%20-%20Ped%20Sb16%20Pos-P%20GO%20Bd16%20M8%20Gb8%20Cornet%20Pos-GO%20Pos%20Um8%20CdN8%20Rec-P%20Rec%20VC8%20VdG8%20Fl4.mp3",

            "https://www.pcorgan.com/mp3/KampenVol3%20-%20Jan%20Mulder%20-%20Vaste%20Rots%20van%20mijn%20behoud%20-%20Koraal,%20divertimento%20en%20Koraal.mp3"
            };
    }


}
