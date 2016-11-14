using Android.App;
using Android.Content.PM;
using MvvmCross.Droid.Views;

namespace MyMediaPlayer.Droid
{
    [Activity(
        Label = "MyMediaPlayer.Droid"
        , MainLauncher = true
        , Icon = "@mipmap/icon"
        , Theme = "@style/AppTheme.Splash"
        , NoHistory = true)]
    public class SplashScreen : MvxSplashScreenActivity
    {
        public SplashScreen()
            : base(Resource.Layout.SplashScreen)
        {
        }
    }
}
