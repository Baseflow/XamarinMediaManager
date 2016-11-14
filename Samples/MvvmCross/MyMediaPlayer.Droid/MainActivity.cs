using Android.App;
using Android.Widget;
using Android.OS;
using MyMediaPlayer.Core.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;

namespace MyMediaPlayer.Droid
{
    [Activity(Label = "MyMediaPlayer.Droid", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : MvxCachingFragmentCompatActivity<HomeViewModel>
    {
        int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
        }
    }
}

