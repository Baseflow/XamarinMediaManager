using Android.App;
using Android.Widget;
using Android.OS;
using Plugin.MediaManager;

namespace ElementPlayer.Droid
{
    [Activity(Label = "ElementPlayer.Droid", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        string url1 = @"http://www.jamesreams.com/wp-content/uploads/2013/01/Born-to-Roll-clip.mp3";
        string url2 = @"http://www.wav-sounds.com/answering_machine/rappin.wav";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate
            { 
                button.Text = $"{count++} clicks!";
                CrossMediaManager.Current.Play(url1, MediaManager.Abstractions.Enums.MediaItemType.Audio);
            };
        }
    }
}

