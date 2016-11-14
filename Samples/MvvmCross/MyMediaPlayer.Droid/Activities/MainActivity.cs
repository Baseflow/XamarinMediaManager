using Android.App;
using Android.Widget;
using Android.OS;
using MyMediaPlayer.Core.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;
using Android.Content.PM;

namespace MyMediaPlayer.Droid
{
    [Activity(Label = "MyMediaPlayer",
              Theme = "@style/AppTheme",
              LaunchMode = LaunchMode.SingleTop,
              MainLauncher = true, 
              Icon = "@mipmap/icon"
             )]
    public class MainActivity : MvxCachingFragmentCompatActivity<MainViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
        }
    }
}

