using Android.App;
using Android.Content.PM;
using Android.OS;
using ElementPlayer.Core.ViewModels;
using Google.Android.Material.BottomNavigation;
using MediaManager;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.Platforms.Android.Views;

namespace ElementPlayer.Android.Activities
{
    [Activity(
        Label = "MainActivity",
        LaunchMode = LaunchMode.SingleTop,
        Theme = "@style/AppTheme"
        )]
    public class MainActivity : MvxActivity<MainViewModel>
    {
        private BottomNavigationView bottomNavigation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_activity);

            CrossMediaManager.Current.Init(this);
            bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
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
