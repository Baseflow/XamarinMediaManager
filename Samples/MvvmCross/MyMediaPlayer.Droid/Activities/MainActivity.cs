using Android.App;
using Android.Widget;
using Android.OS;
using MyMediaPlayer.Core.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Renderscripts;
using Android.Graphics;
using Android.Graphics.Drawables;

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
        private BottomSheetBehavior mBottomSheetBehavior;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var parentThatHasBottomSheetBehavior = FindViewById<FrameLayout>(Resource.Id.bottomsheetview);
            mBottomSheetBehavior = BottomSheetBehavior.From(parentThatHasBottomSheetBehavior);
            if (mBottomSheetBehavior != null)
            {
                mBottomSheetBehavior.SetBottomSheetCallback(new PlayerBehavior(Window.DecorView.RootView));
            }

            var miniPlayer = FindViewById<RelativeLayout>(Resource.Id.miniplayerview);
            miniPlayer.Click += (sender, e) =>
            {
                mBottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
            };
        }

        private class PlayerBehavior : BottomSheetBehavior.BottomSheetCallback
        {
            View screenView;

            public PlayerBehavior(View view)
            {
                screenView = view;
            }

            public override void OnSlide(View bottomSheet, float newState)
            {
                var toolbar = screenView.FindViewById<AppBarLayout>(Resource.Id.appbar);
                toolbar.Alpha = 1 - newState;

                var playertoolbar = screenView.FindViewById<AppBarLayout>(Resource.Id.playerappbar);
                playertoolbar.Alpha = newState;

                var miniplayer = screenView.FindViewById<View>(Resource.Id.miniplayerview);
                miniplayer.Alpha = 1 - newState;
            }

            public override void OnStateChanged(View bottomSheet, int slideOffset)
            {
            }
        }
    }
}

