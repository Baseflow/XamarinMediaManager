using Android.OS;
using Android.Views;
using Com.Google.Android.Exoplayer2.UI;
using ElementPlayer.Core.ViewModels;
using MediaManager;
using MediaManager.Platforms.Android.Video;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace ElementPlayer.Android.Fragments
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.content_frame, false)]
    public class MiniPlayerFragment : BaseFragment<MiniPlayerViewModel>
    {
        protected override int FragmentId => Resource.Layout.player_fragment;
    }
}
