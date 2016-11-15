
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using MvvmCross.Droid.Shared.Attributes;
using MyMediaPlayer.Core.ViewModels;

namespace MyMediaPlayer.Droid.Fragments
{
    [MvxFragment(typeof(MainViewModel), Resource.Id.content_frame, true)]
    [Register("mymediaplayer.droid.fragments.HomeFragment")]
    public class HomeFragment : BaseFragment<HomeViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_home;
        private BottomSheetBehavior mBottomSheetBehavior;

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var parentThatHasBottomSheetBehavior = view.FindViewById<FrameLayout>(Resource.Id.bottomsheetview);
            mBottomSheetBehavior = BottomSheetBehavior.From(parentThatHasBottomSheetBehavior);
            if (mBottomSheetBehavior != null)
            {
                mBottomSheetBehavior.SetBottomSheetCallback(new PlayerBehavior(mBottomSheetBehavior, view));
            }
        }

        private class PlayerBehavior : BottomSheetBehavior.BottomSheetCallback
        {
            BottomSheetBehavior bottomSheetBehavior;
            View screenView;

            public PlayerBehavior(BottomSheetBehavior bottom, View view)
            {
                bottomSheetBehavior = bottom;
                screenView = view;
            }

            public override void OnSlide(View bottomSheet, float newState)
            {
                
            }

            public override void OnStateChanged(View bottomSheet, int slideOffset)
            {
                var toolbar = screenView.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

                switch ((bottomSheetBehavior.State))
                {
                    case BottomSheetBehavior.StateCollapsed:
                        toolbar.Visibility = ViewStates.Visible;
                        break;
                    case BottomSheetBehavior.StateDragging:
                        break;
                    case BottomSheetBehavior.StateExpanded:
                        break;
                    case BottomSheetBehavior.StateHidden:
                        break;
                    case BottomSheetBehavior.StateSettling:
                        toolbar.Visibility = ViewStates.Gone;
                        break;
                }
            }
        }
    }
}
