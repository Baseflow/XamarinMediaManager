using ElementPlayer.Core.ViewModels;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace ElementPlayer.Android.Fragments
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.content_frame, false)]
    public class PlayerFragment : BaseFragment<PlayerViewModel>
    {
        protected override int FragmentId => Resource.Layout.player_fragment;
    }
}
