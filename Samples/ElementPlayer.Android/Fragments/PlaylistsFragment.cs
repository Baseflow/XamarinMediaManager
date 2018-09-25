using ElementPlayer.Core.ViewModels;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace ElementPlayer.Android.Fragments
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.content_frame, false)]
    public class PlaylistsFragment : BaseFragment<PlaylistsViewModel>
    {
        protected override int FragmentId => Resource.Layout.playlists_fragment;
    }
}
