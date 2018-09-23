using Android.App;
using Android.OS;
using ElementPlayer.Core;
using ElementPlayer.Core.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace ElementPlayer.Android.Views
{
    [MvxActivityPresentation]
    [Activity(
        Label = "@string/ApplicationName", 
        Theme = "@style/AppTheme")]
    public class HomeView : MvxAppCompatActivity<HomeViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.HomeView);
        }
    }
}
