using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using ElementPlayer.Core.ViewModels;
using MvvmCross;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Navigation;

namespace ElementPlayer.Android.Activities
{
    [Activity(
        Label = "MainActivity",
        LaunchMode = LaunchMode.SingleTop,
        Theme = "@style/AppTheme"
        )]
    public class MainActivity : MvxAppCompatActivity<MainViewModel>
    {
        BottomNavigationView bottomNavigation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_activity);

            bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected; ;
        }

        private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.Item.ItemId)
            {
                case Resource.Id.menu_home:
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<HomeViewModel>();
                    break;
                case Resource.Id.menu_browse:
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<BrowseViewModel>();
                    break;
                case Resource.Id.menu_search:
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<SearchViewModel>();
                    break;
                case Resource.Id.menu_playlists:
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<PlaylistsViewModel>();
                    break;
                case Resource.Id.menu_settings:
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<SettingsViewModel>();
                    break;
                default:
                    break;
            }
        }
    }
}
