
using Android.App;
using Android.OS;

using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;

namespace ElementPlayer.Android.Views
{
    [MvxActivityPresentation]
    [Activity(Label = "@string/ApplicationName")]
    public class HomeView : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.HomeView);
        }
    }
}
