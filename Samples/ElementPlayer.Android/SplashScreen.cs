using Android.App;
using Android.Content.PM;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Views;

namespace ElementPlayer.Android
{
    [Activity(Label = "@string/ApplicationName",
        MainLauncher = true,
        Icon = "@drawable/btn_play_active",
        RoundIcon = "@drawable/btn_play_active",
        Theme = "@style/MainTheme",
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class SplashScreen : MvxSplashScreenActivity<MvxAndroidSetup<Core.App>, Core.App>
    {
        public SplashScreen()
             : base(Resource.Layout.SplashScreen)
        {
        }
    }
}
