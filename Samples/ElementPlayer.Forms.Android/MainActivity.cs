
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using MediaManager;
using MvvmCross.Forms.Platforms.Android.Views;

namespace ElementPlayer.Forms.Droid
{
    [Activity(
        Label = "ElementPlayer.Forms",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : MvxFormsAppCompatActivity<Setup, Core.App, FormsApp>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Droid.Resource.Layout.Tabbar;
            ToolbarResource = Droid.Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            CrossMediaManager.Current.Init(this);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
