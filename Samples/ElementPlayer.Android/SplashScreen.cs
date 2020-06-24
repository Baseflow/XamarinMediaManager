using Android.App;
using Android.Content.PM;
using MvvmCross.Core;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Views;

namespace ElementPlayer.Android
{
    [Activity(Label = "@string/ApplicationName",
        MainLauncher = true,
        NoHistory = true,
        Icon = "@drawable/baseline_play_circle_filled_24",
        RoundIcon = "@drawable/baseline_play_circle_filled_24",
        Theme = "@style/AppTheme.Splash",
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class SplashScreen : MvxSplashScreenActivity<MvxAndroidSetup<Core.App>, Core.App>
    {
        public SplashScreen()
             : base(Resource.Layout.splash_screen)
        {
        }
    }
}
