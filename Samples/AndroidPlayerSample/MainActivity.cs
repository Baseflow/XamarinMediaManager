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
            "https://www.pcorgan.com/mp3/Caen%20-%20Jan%20Mulder%20-%20Wie%20maar%20de%20goede%20God%20laat%20zorgen%20-%20Toccata%20-%20Koraal%20-%20mf-fff.mp3",
            "https://www.pcorgan.com/mp3/Kampen%20II%20-%20Feike%20Asma%20-%20Ps%2072%20-%20Ped%20Pr16%20Sb16%20Oct8%20Ged8%20Oct4%20Bw-P%20Rw%20Sexq%20Fl4%20Hp8%20Pr8%20Oct4%20Q3%20Fl2%20Hw%20Tert%20Pr8%20Oct4%20Q3%20Oct2%20Tr8%20Rw-Hw%20Bw%20All-Car-Fl1-Dulc-Tr8.mp3",
            "https://www.pcorgan.com/mp3/Kampen3%20-%20Peter%20de%20Wilde%20-%20Ps%20121v1%20-%20A%20Bd16%20Hp8%20Fl4%20B%20Ped%20Sb16%20Gd8%20Hw-P%20Rw%20Hp8%20GdQ3%20Trem%20Hw%20Hp8%20Fl4%20Bw%20Tr8.mp3",
            "https://www.pcorgan.com/mp3/Caen%20-%20Marco%20den%20Toom%20-%20Psalm%20134%20-%20Ped%20Sb16%20Pos-P%20GO%20Bd16%20M8%20Gb8%20Cornet%20Pos-GO%20Pos%20Um8%20CdN8%20Rec-P%20Rec%20VC8%20VdG8%20Fl4.mp3",

            "https://www.pcorgan.com/mp3/Caen%20Surround%20-%20Feike%20Asma%20-%20Wat%20de%20toekomst%20brenge%20moge%20-%20Ped%20Soub16%20Viol8%20R-P%20GO%20Gamb8%20Bd8%20Fl4%20R-GO%20Pos%20Cromorne%20Rec%20VdG8%20VC8.mp3"
            };
    }


}